using cAlgo.API.Internals;

namespace cAlgo.API.Ext.Chart;

public static class ConditionExtensions
{
    /// <summary>
    /// 手動で追加した HorizontalLine か判定する。
    /// </summary>
    /// <param name="horizontalLine"></param>
    /// <returns></returns>
    public static bool IsAddedManually(this ChartHorizontalLine horizontalLine)
    {
        const string defaultName = "Horizontal Line";
        return horizontalLine.Name.StartsWith(defaultName);
    }
    // IsAddedManually

    /// <summary>
    /// TrendLine が水平部分線か判定する。
    /// </summary>
    /// <param name="trendLine"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsHorizontalPartialLine(this ChartTrendLine trendLine, Symbol symbol)
    {
        var distance = Math.Round(
            value: Math.Abs(trendLine.Y1 - trendLine.Y2),
            digits: 5,
            mode: MidpointRounding.AwayFromZero);

        var distanceInPips = Math.Round(
            value: distance / symbol.PipSize,
            digits: symbol.Digits,
            mode: MidpointRounding.AwayFromZero);

        // TODO TimeFrame に応じて閾値を変えた方がいいかも？

        const int baseThresholdPips = 10;
        var threshold = baseThresholdPips;
        if (symbol.Name == "XAUUSD")
        {
            threshold = baseThresholdPips * 40;
        }

        return distanceInPips < threshold;
    }

    /// <summary>
    /// EconomicEvents の Line か判定する。
    /// </summary>
    /// <param name="verticalLine"></param>
    /// <returns></returns>
    public static bool IsEconomicEventsLine(this ChartVerticalLine verticalLine)
    {
        return verticalLine.Name.StartsWith("HIGH") || verticalLine.Name.StartsWith("MEDIUM");
    }
}