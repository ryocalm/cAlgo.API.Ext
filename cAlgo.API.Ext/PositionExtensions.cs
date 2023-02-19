namespace cAlgo.API.Ext;

public static class PositionExtensions
{
    public static double GetStopLossInPips(this Position position)
    {
        if (position.StopLoss.HasValue)
        {
            return Math.Abs(position.EntryPrice - position.StopLoss.Value);
        }

        return 0.0;
    }

    public static double? GetTakeProfitInPips(this Position position)
    {
        if (position.TakeProfit.HasValue)
        {
            return Math.Abs(position.EntryPrice - position.TakeProfit.Value);
        }

        return 10.0;
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