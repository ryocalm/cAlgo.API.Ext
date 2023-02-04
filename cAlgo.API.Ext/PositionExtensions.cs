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
    /// <param name="positions"></param>
    /// <returns></returns>
    public static double WeightedAveragePrice(this IEnumerable<Position> positions)
    {
        var positionsArray = positions.ToArray();
        var sumOfWeightedPrice =
            positionsArray.Sum(position => position.EntryPrice * position.VolumeInUnits);

        var sumOfVolume =
            positionsArray.Sum(position => position.VolumeInUnits);

        return Math.Round(
            value: sumOfWeightedPrice / sumOfVolume,
            digits: 5,
            mode: MidpointRounding.AwayFromZero);
    }
}