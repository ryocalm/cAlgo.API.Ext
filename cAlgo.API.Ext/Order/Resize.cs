using cAlgo.API.Internals;

namespace cAlgo.API.Ext.Order;

public class ResizePlan
{
    public readonly double StopLossPrice;
    public readonly double EachPositionSize;
    public double MarketOrderStopLossPips;

    // totalPositionSize が VolumeInUnitsMin 以下になる場合、
    // Lot を VolumeInUnitsMin にして TargetPrice の方を変更する。

    /// <summary>
    /// TargetPrice を StopLossPrice に近づける。単位は price と同じ。
    /// </summary>
    public double TargetPriceOffset;

    public ResizePlan(double stopLossPrice, double eachPositionSize)
    {
        StopLossPrice = stopLossPrice;
        EachPositionSize = eachPositionSize;
    }
}

public static class Resize
{
    /// <summary>
    /// Pending Orders の StopLoss price の平均を計算する。
    /// </summary>
    /// <param name="pendingOrders"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static double GetAverageStopLossPrice(
        IReadOnlyCollection<PendingOrder> pendingOrders)
    {
        if (!pendingOrders.Any())
        {
            throw new Exception("No pendingOrders...");
        }

        // Buy / Sell いずれかに揃っている必要がある。
        if (pendingOrders.Any(order => order.TradeType == TradeType.Buy)
            && pendingOrders.Any(order => order.TradeType == TradeType.Sell))
        {
            throw new Exception("Contains both Buy and Sell orders...");
        }

        // StopLoss が設定されていない場合は全ての order をキャンセルする。
        if (pendingOrders.Any(order => order.StopLoss == null))
        {
            foreach (var order in pendingOrders)
            {
                order.Cancel();
            }

            throw new Exception("Every PendingOrder MUST be set StopLoss!");
        }

        var stopLossPrices = pendingOrders
            .Select(order => order.StopLoss)
            .Where(stopLoss => stopLoss != null)
            .ToArray();

        switch (stopLossPrices.Length)
        {
            case 1:
            {
                var stopLoss = stopLossPrices.Single();
                if (stopLoss.HasValue)
                {
                    return stopLoss.Value;
                }

                break;
            }
            default:
            {
                var average = stopLossPrices.Average();
                if (average.HasValue)
                {
                    return average.Value;
                }

                break;
            }
        }

        const double dummyAverageStopLoss = 10.0;
        return dummyAverageStopLoss;
    }

    /// <summary>
    /// PendingOrder の ResizePlan を生成する。
    /// </summary>
    /// <param name="pendingOrders"></param>
    /// <param name="balance"></param>
    /// <param name="allowableLossRate">0.5% など</param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ResizePlan GetResizePlanOfPendingOrders(
        PendingOrder[] pendingOrders,
        double balance,
        double allowableLossRate,
        Symbol symbol)
    {
        var averageTargetPrice = pendingOrders
            .Select(order => order.TargetPrice)
            .Average();

        var stopLossPrice = GetAverageStopLossPrice(pendingOrders);

        var priceRangePips = symbol.GetDistanceInPips(
            from: averageTargetPrice,
            to: stopLossPrice);

        // 1 トレードにおける許容損失金額
        // 例 : balance 100,000 円の 0.5% で 500 円
        var allowableRiskAmount = balance * allowableLossRate / 100;

        // 1 pip あたりの許容損失金額
        // 例 : USD/JPY で 500 円を 10 pips で割ると 50 円/pips
        // 例 : XAU/USD で 500 円を 500 pips で割ると 1 円/pips
        var riskAmountPerPips = Math.Round(
            value: allowableRiskAmount / priceRangePips,
            digits: 1,
            mode: MidpointRounding.AwayFromZero);

        // VolumeInUnits
        // 例 : USD/JPY で 50 円を pipValue 0.01 で割ると 5,000 Units (0.05 Lot)
        // 例 : XAU/USD で 1 円を pipValue 1.10 で割ると 0.909 Units (0.01 Lot) 弱
        var totalPositionSize = riskAmountPerPips / symbol.PipValue;

        if (totalPositionSize / pendingOrders.Length > symbol.VolumeInUnitsMin)
        {
            var eachPositionSize = symbol.NormalizeVolumeInUnits(totalPositionSize / pendingOrders.Length);
            return new ResizePlan(
                stopLossPrice: stopLossPrice,
                eachPositionSize: eachPositionSize);
        }

        // VolumeInUnitsMin に満たない場合
        // stopLossPrice は動かさない
        // TargetPrice を動かす（StopLossPrice に近づける）
        // riskAmountPips を変更する必要がある。

        var resizePlan = new ResizePlan(
            stopLossPrice: stopLossPrice,
            eachPositionSize: symbol.VolumeInUnitsMin);

        // 1 * 1.10 = 1.10
        var validRiskAmountPerPips =
            symbol.VolumeInUnitsMin * symbol.PipValue;

        // 500 円を 1.10 で割ると 454.5 pips
        var validPriceRangePips =
            Math.Floor(allowableRiskAmount / validRiskAmountPerPips);

        resizePlan.TargetPriceOffset =
            Math.Abs(validPriceRangePips - priceRangePips) * symbol.PipSize;

        return resizePlan;
    }

    /// <summary>
    /// MarketOrder の ResizePlan を生成する。
    /// </summary>
    /// <param name="pendingOrder"></param>
    /// <param name="targetPrice">Bid or Ask</param>
    /// <param name="balance"></param>
    /// <param name="allowableLossRate"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ResizePlan GetResizePlanOfMarketOrder(
        PendingOrder pendingOrder,
        double targetPrice,
        double balance,
        double allowableLossRate,
        Symbol symbol)
    {
        double stopLossPrice = 0;
        if (pendingOrder.StopLoss.HasValue)
        {
            stopLossPrice = pendingOrder.StopLoss.Value;
        }

        var priceRangePips = symbol.GetDistanceInPips(
            from: targetPrice,
            to: stopLossPrice);

        // 1 トレードにおける許容損失金額
        // 例 : balance 100,000 円の 0.5% で 500 円
        var allowableRiskAmount = balance * allowableLossRate / 100;

        // 1 pip あたりの許容損失金額
        // 例 : USD/JPY で 500 円を 10 pips で割ると 50 円/pips
        // 例 : XAU/USD で 500 円を 500 pips で割ると 1 円/pips
        var riskAmountPerPips = Math.Round(
            value: allowableRiskAmount / priceRangePips,
            digits: 1,
            mode: MidpointRounding.AwayFromZero);

        // VolumeInUnits
        // 例 : USD/JPY で 50 円を pipValue 0.01 で割ると 5,000 Units (0.05 Lot)
        // 例 : XAU/USD で 1 円を pipValue 1.10 で割ると 0.909 Units (0.01 Lot) 弱
        var totalPositionSize = riskAmountPerPips / symbol.PipValue;

        var eachPositionSize = totalPositionSize > symbol.VolumeInUnitsMin
            ? symbol.NormalizeVolumeInUnits(totalPositionSize)
            : symbol.VolumeInUnitsMin;

        var resizePlan = new ResizePlan(stopLossPrice, eachPositionSize)
        {
            MarketOrderStopLossPips = priceRangePips
        };
        return resizePlan;
    }
}