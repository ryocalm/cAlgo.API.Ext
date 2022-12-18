using cAlgo.API.Internals;

namespace cAlgo.API.Ext.Chart;

public static class SetColor
{
    /// <summary>
    /// Chart の基本的な配色を設定する。
    /// </summary>
    /// <param name="chart"></param>
    public static void ChartColorSettings(API.Chart chart)
    {
        var cs = chart.ColorSettings;
        cs.BackgroundColor = Colors.Background;
        cs.ForegroundColor = Colors.Foreground;
        cs.GridLinesColor = Colors.GridLines;
        cs.PeriodSeparatorColor = Colors.PeriodSeparator;

        // .spa などの postfix がついている場合があるので通貨ペア名だけを取る。
        var ccyPair = chart.SymbolName.Substring(0, 6);
        // USD/JPY の場合 USD
        var baseCcy = ccyPair.Substring(0, 3);
        // USD/JPY の場合 JPY
        var quoteCcy = ccyPair.Substring(3, 3);

        cs.BullOutlineColor = Colors.QueryCcyColor(baseCcy);
        cs.BearOutlineColor = Colors.QueryCcyColor(quoteCcy);

        cs.BullFillColor = Color.Transparent;
        cs.BearFillColor = Color.Transparent;

        cs.WinningDealColor = Colors.WinningDeal;
        cs.LosingDealColor = Colors.LosingDeal;

        cs.AskPriceLineColor = Colors.AskPriceLine;
        cs.BidPriceLineColor = Colors.BidPriceLine;

        cs.BuyColor = Colors.Buy;
        cs.SellColor = Colors.Sell;
    }

    /// <summary>
    /// HorizontalLine の Color を設定する
    /// </summary>
    /// <param name="horizontalLine"></param>
    /// <param name="ask"></param>
    public static void HorizontalLine(ChartHorizontalLine horizontalLine, double ask)
    {
        // 手動で追加したラインのみを対象とする。
        if (!Conditions.StartsWithDefaultName(horizontalLine))
        {
            return;
        }

        horizontalLine.Color = horizontalLine.Y >= ask ? Colors.ResistanceLine : Colors.SupportLine;
    }

    /// <summary>
    /// VerticalLine の Color を設定する
    /// </summary>
    /// <param name="verticalLine"></param>
    public static void VerticalLine(ChartVerticalLine verticalLine)
    {
    }

    /// <summary>
    /// TrendLine の Color を設定する。
    /// </summary>
    /// <param name="trendLine"></param>
    /// <param name="symbol"></param>
    /// <param name="ask"></param>
    public static void TrendLine(ChartTrendLine trendLine, Symbol symbol, double ask)
    {
        if (Conditions.IsHorizontalPartialLine(trendLine, symbol))
        {
            trendLine.Color = trendLine.Y1 >= ask ? Colors.ResistanceLine : Colors.SupportLine;
        }
        else
        {
            // 水平部分線ではない場合
            trendLine.Color = Color.LightGray;
        }
    }

    /// <summary>
    /// Rectangle の色を設定する
    /// </summary>
    /// <param name="rectangle"></param>
    /// <param name="bars">Rectangle の上下にある Bar</param>
    /// <param name="ask">Ask</param>
    /// <param name="bid">Bid</param>
    public static void Rectangle(ChartRectangle rectangle, Bar[] bars, double ask, double bid)
    {
        // 安値マーカー
        // 各 Bar の Low が上辺（Y1）より上にある
        if (bars.All(bar => bar.Low > rectangle.Y1))
        {
            rectangle.Color = Colors.SupportZone;
            return;
        }

        // 高値マーカー
        // 各 Bar の High が下辺（Y2）より下にある
        if (bars.All(bar => bar.High < rectangle.Y2))
        {
            rectangle.Color = Colors.ResistanceZone;
            return;
        }

        // 以下、マーカーではない場合

        // 過去のものは Color を変更しない。
        if (rectangle.Time2 < DateTime.UtcNow) return;

        if (rectangle.Y1 <= bid)
        {
            rectangle.Color = Colors.SupportZone;
            return;
        }

        if (rectangle.Y2 >= ask)
        {
            rectangle.Color = Colors.ResistanceZone;
        }
    }

    /// <summary>
    /// Ellipse の Color を設定する。
    /// </summary>
    public static void Ellipse(ChartEllipse ellipse)
    {
        ellipse.Color = Colors.LineMarker;
    }

    /// <summary>
    /// Triangle の Color を設定する。
    /// </summary>
    /// <param name="triangle"></param>
    public static void Triangle(ChartTriangle triangle)
    {
        var middleY = (triangle.Y2 + triangle.Y3) / 2;
        triangle.Color = triangle.Y1 > middleY ? Colors.TriangleA : Colors.TriangleV;
    }

    /// <summary>
    /// ChartText の Color を設定する。
    /// </summary>
    /// <param name="text"></param>
    public static void Text(ChartText text)
    {
        text.Color = Color.LightGray;
    }

    /// <summary>
    /// ChartStaticText の Color を設定する。
    /// </summary>
    /// <param name="staticText"></param>
    public static void StaticText(ChartStaticText staticText)
    {
        staticText.Color = Color.LightGray;
    }

    /// <summary>
    /// ChartIcon の Color を設定する。
    /// </summary>
    /// <param name="icon"></param>
    public static void Icon(ChartIcon icon)
    {
        switch (icon.IconType)
        {
            case ChartIconType.UpArrow:
            case ChartIconType.UpTriangle:
                icon.Color = Colors.SupportLine;
                break;
            case ChartIconType.DownArrow:
            case ChartIconType.DownTriangle:
                icon.Color = Colors.ResistanceLine;
                break;
        }
    }
}