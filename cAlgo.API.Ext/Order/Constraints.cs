namespace cAlgo.API.Ext.Order;

public static class Constraints
{
    /// <summary>
    /// Long エントリーがサポートラインからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="horizontalLines"></param>
    /// <returns></returns>
    private static bool IsLongEntryFromSupportLine(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartHorizontalLine> horizontalLines)
    {
        // targetPrice が SupportLine よりも thresholdPips 分だけ
        // 上に離れていても許容する。
        // つまり適切かどうかを判定する price は targetPrice よりも下になる。
        var threshold = thresholdPips * pipSize;
        var thresholdPrice = targetPrice - threshold;

        // SupportLine の上下 thresholdPips 分までを許容する。
        // targetPrice が Line よりも下にある場合も無制限に許容すると、
        // 制約の意味を成さなくなる。
        // http://axiory.ctrader.com/c/jd8vn
        var positiveThresholdPrice = targetPrice + threshold;

        return horizontalLines
            .Where(hLine => hLine.Color == Colors.SupportLine)
            .Where(hLine => hLine.Y >= thresholdPrice)
            .Any(hLine => hLine.Y <= positiveThresholdPrice);
    }

    /// <summary>
    /// Short エントリーがレジスタンスラインからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="horizontalLines"></param>
    /// <returns></returns>
    private static bool IsShortEntryFromResistanceLine(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartHorizontalLine> horizontalLines)
    {
        // targetPrice が ResistanceLine よりも thresholdPips 分だけ
        // 下に離れていても許容する。
        // つまり適切かどうかを判定する price は targetPrice よりも上になる。
        var threshold = thresholdPips * pipSize;
        var thresholdPrice = targetPrice + threshold;

        // ResistanceLine の上下 thresholdPips 分までを許容する。
        // targetPrice が Line よりも上にある場合も無制限に許容すると、
        // 制約の意味を成さなくなる。
        // http://axiory.ctrader.com/c/4d8vn
        var positiveThresholdPrice = targetPrice - threshold;

        return horizontalLines
            .Where(hLine => hLine.Color == Colors.ResistanceLine)
            .Where(hLine => hLine.Y <= thresholdPrice)
            .Any(hLine => hLine.Y >= positiveThresholdPrice);
    }

    /// <summary>
    /// Long エントリーがサポートゾーンからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsLongEntryFromSupportZone(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartRectangle> rectangles)
    {
        // targetPrice が SupportZone よりも thresholdPips 分だけ
        // 上に離れていても許容する。
        // つまり適切かどうかを判定する price は targetPrice よりも下になる。
        var thresholdPrice = targetPrice - thresholdPips * pipSize;

        // 上辺から下辺の間、もしくは上辺 + threshold 分までを許容する。
        // 1. 上辺が targetPrice よりも下にある場合
        // 2. 上辺が targetPrice よりも上にある場合（= Rectangle の中に targetPrice がある）
        // いずれも下辺は targetPrice よりも下にあるべき。（= 突き抜けている場合は対象外とすべき）
        // http://axiory.ctrader.com/c/zd8vn

        // - 上辺は thresholdPrice と同じか上にあるべき。
        // - 下辺は targetPrice と同じか下にあるべき。
        return rectangles
            .Where(rectangle => rectangle.Color == Colors.SupportZone)
            .Where(rectangle =>
            {
                var upperSide = rectangle.Y1 > rectangle.Y2 ? rectangle.Y1 : rectangle.Y2;
                // var upperSide = rectangle.Y1;
                // if (rectangle.Y1 < rectangle.Y2) upperSide = rectangle.Y2;
                return upperSide >= thresholdPrice;
            })
            .Any(rectangle =>
            {
                var lowerSide = rectangle.Y1 > rectangle.Y2 ? rectangle.Y2 : rectangle.Y1;
                // var lowerSide = rectangle.Y2;
                // if (rectangle.Y1 < rectangle.Y2) lowerSide = rectangle.Y1;
                return lowerSide <= targetPrice;
            });
    }

    /// <summary>
    /// Short エントリーがレジスタンスゾーンからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsShortEntryFromResistanceZone(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartRectangle> rectangles)
    {
        // targetPrice が ResistanceZone よりも thresholdPips 分だけ
        // 下に離れていても許容する。
        // つまり適切かどうかを判定する price は targetPrice よりも上になる。
        var thresholdPrice = targetPrice + thresholdPips * pipSize;

        // 上辺から下辺の間、もしくは下辺 - threshold 分までを許容する。
        // 1. 下辺が targetPrice よりも上にある場合（= Rectangle の下に targetPrice がある）
        // 2. 下辺が targetPrice よりも下にある場合（= Rectangle の中に targetPrice がある）
        // いずれも上辺は targetPrice よりも上にあるべき。（= 突き抜けている場合は対象外とすべき）
        // http://axiory.ctrader.com/c/8d8vn

        // - 上辺は targetPrice と同じか上にあるべき。
        // - 下辺は thresholdPrice と同じか下にあるべき。
        return rectangles
            .Where(rectangle => rectangle.Color == Colors.ResistanceZone)
            .Where(rectangle =>
            {
                var upperSide = rectangle.Y1 > rectangle.Y2 ? rectangle.Y1 : rectangle.Y2;
                return upperSide >= targetPrice;
            })
            .Any(rectangle =>
            {
                var lowerSide = rectangle.Y2;
                if (rectangle.Y1 < rectangle.Y2) lowerSide = rectangle.Y1;
                return lowerSide <= thresholdPrice;
            });
    }

    /// <summary>
    /// Long エントリーがサポートライン・ゾーンからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="horizontalLines"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsLongEntryFromSupport(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartHorizontalLine> horizontalLines,
        IEnumerable<ChartRectangle> rectangles)
    {
        return IsLongEntryFromSupportLine(
                   targetPrice: targetPrice,
                   thresholdPips: thresholdPips,
                   pipSize: pipSize,
                   horizontalLines: horizontalLines)
               || IsLongEntryFromSupportZone(
                   targetPrice: targetPrice,
                   thresholdPips: thresholdPips,
                   pipSize: pipSize,
                   rectangles: rectangles);
    }

    /// <summary>
    /// Short エントリーがレジスタンスライン・ゾーンからのものか判定する。
    /// </summary>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="horizontalLines"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsShortEntryFromResistance(
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartHorizontalLine> horizontalLines,
        IEnumerable<ChartRectangle> rectangles)
    {
        return IsShortEntryFromResistanceLine(
                   targetPrice: targetPrice,
                   thresholdPips: thresholdPips,
                   pipSize: pipSize,
                   horizontalLines: horizontalLines)
               || IsShortEntryFromResistanceZone(
                   targetPrice: targetPrice,
                   thresholdPips: thresholdPips,
                   pipSize: pipSize,
                   rectangles: rectangles);
    }

    /// <summary>
    /// エントリーがサポート・レジスタンスからのものか判定する。
    /// </summary>
    /// <param name="tradeType"></param>
    /// <param name="targetPrice"></param>
    /// <param name="thresholdPips"></param>
    /// <param name="pipSize"></param>
    /// <param name="horizontalLines"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsEntryFromSupportOrResistance(
        TradeType tradeType,
        double targetPrice,
        double thresholdPips,
        double pipSize,
        IEnumerable<ChartHorizontalLine> horizontalLines,
        IEnumerable<ChartRectangle> rectangles)
    {
        return tradeType switch
        {
            TradeType.Buy => IsLongEntryFromSupport(
                targetPrice, thresholdPips, pipSize, horizontalLines, rectangles),
            TradeType.Sell => IsShortEntryFromResistance(
                targetPrice, thresholdPips, pipSize, horizontalLines, rectangles),
            _ => throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null)
        };
    }

    /// <summary>
    /// 上方の Rectangle の有無をチェックする。
    /// （＝ Long の利確目標の帯を引いているか）
    /// </summary>
    /// <param name="askPrice"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsRectangleAbove(double askPrice, IEnumerable<ChartRectangle> rectangles)
    {
        return rectangles
            .Where(rectangle => rectangle.Color == Colors.ResistanceZone)
            .Any(rectangle =>
            {
                var lowerSide = rectangle.Y2;
                if (rectangle.Y1 < rectangle.Y2) lowerSide = rectangle.Y1;
                return askPrice < lowerSide;
            });
    }

    /// <summary>
    /// 下方の Rectangle の有無をチェックする。
    /// （＝ Short の利確目標の帯を引いているか）
    /// </summary>
    /// <param name="bidPrice"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    private static bool IsRectangleBelow(double bidPrice, IEnumerable<ChartRectangle> rectangles)
    {
        return rectangles
            .Where(rectangle => rectangle.Color == Colors.SupportZone)
            .Any(rectangle =>
            {
                var upperSide = rectangle.Y1 > rectangle.Y2 ? rectangle.Y1 : rectangle.Y2;
                return upperSide < bidPrice;
            });
    }

    /// <summary>
    /// 上方、下方の Rectangle の有無をチェックする
    /// </summary>
    /// <param name="tradeType"></param>
    /// <param name="askPrice"></param>
    /// <param name="bidPrice"></param>
    /// <param name="rectangles"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsRectangleAboveOrBelow(
        TradeType tradeType,
        double askPrice,
        double bidPrice,
        IEnumerable<ChartRectangle> rectangles)
    {
        return tradeType switch
        {
            TradeType.Buy => IsRectangleAbove(askPrice, rectangles),
            TradeType.Sell => IsRectangleBelow(bidPrice, rectangles),
            _ => throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null)
        };
    }
}