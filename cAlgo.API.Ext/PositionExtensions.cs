using cAlgo.API.Internals;

namespace cAlgo.API.Ext;

public static class PositionExtensions
{
    public static double? GetStopLossInPips(this Position position, Symbol symbol)
    {
        var stopLossPips = position.TradeType == TradeType.Buy
            ? position.EntryPrice - position.StopLoss
            : position.StopLoss - position.EntryPrice;
        return stopLossPips ?? 10.0;
    }

    public static double? GetTakeProfitInPips(this Position position, Symbol symbol)
    {
        var takeProfitPips = position.TradeType == TradeType.Buy
            ? position.TakeProfit - position.EntryPrice
            : position.EntryPrice - position.TakeProfit;
        return takeProfitPips ?? 10.0;
    }

    /// <summary>
    /// 複数のポジションの加重平均 price を求める。
    /// </summary>
    /// <param name="rawPositions"></param>
    /// <returns></returns>
    public static double WeightedAveragePrice(this IEnumerable<Position> rawPositions)
    {
        var positions = rawPositions.ToArray();
        var weightedPriceSum =
            positions.Sum(position => position.EntryPrice * position.VolumeInUnits);

        var volumeSum =
            positions.Sum(position => position.VolumeInUnits);

        return Math.Round(
            value: weightedPriceSum / volumeSum,
            digits: 5,
            mode: MidpointRounding.AwayFromZero);
    }
}