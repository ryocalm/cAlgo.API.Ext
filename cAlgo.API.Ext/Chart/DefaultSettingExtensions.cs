using cAlgo.API.Internals;

namespace cAlgo.API.Ext.Chart;

public static class DefaultSettingExtensions
{
    /// <summary>
    /// HorizontalLine のデフォルト設定
    /// </summary>
    /// <param name="horizontalLine"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartHorizontalLine horizontalLine, TimeFrame timeFrame)
    {
        // horizontalLine.Comment ??= timeFrame.ToString();
        if (horizontalLine.Comment == "")
        {
            horizontalLine.Comment = timeFrame.ToString();
            //horizontalLine.
        }

        // 手動で追加したラインのみを対象とする。
        if (!horizontalLine.IsAddedManually())
        {
            return;
        }

        horizontalLine.LineStyle = LineStyle.DotsRare;

        // TimeFrame に応じて Thickness を変更する。
        // TODO 描画された TimeFrame によって分岐させたほうがよいかも？
        // if (timeFrame > TimeFrame.Hour)
        // {
        //     horizontalLine.Thickness = 3;
        // }
        // else
        // {
        //     horizontalLine.Thickness = 2;
        // }
        horizontalLine.Thickness = 2;
    }

    /// <summary>
    /// VerticalLine のデフォルト設定
    /// </summary>
    /// <param name="verticalLine"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartVerticalLine verticalLine, TimeFrame timeFrame)
    {
        verticalLine.Comment ??= timeFrame.ToString();
    }

    /// <summary>
    /// TrendLine のデフォルト設定
    /// </summary>
    /// <param name="trendLine"></param>
    /// <param name="timeFrame"></param>
    /// <param name="symbol"></param>
    public static void SetDefault(this ChartTrendLine trendLine, TimeFrame timeFrame, Symbol symbol)
    {
        trendLine.Comment ??= timeFrame.ToString();

        trendLine.Thickness = 1;
        trendLine.LineStyle = LineStyle.DotsRare;

        // 水平化
        if (trendLine.IsHorizontalPartialLine(symbol))
        {
            trendLine.Y2 = trendLine.Y1;
        }
    }

    /// <summary>
    /// Rectangle のデフォルト設定
    /// </summary>
    /// <param name="rectangle"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartRectangle rectangle, TimeFrame timeFrame)
    {
        rectangle.Comment ??= timeFrame.ToString();

        rectangle.IsFilled = true;

        // 右辺を現在よりも未来の時間にした場合に揃える
        var n = DateTime.UtcNow;
        var now = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, 0);

        const double margin = 1.0;
        const double increment = 4.0;

        // 無限ループを防ぐ
        if (rectangle.Time2 > now.AddHours(margin)
            && rectangle.Time2 != now.AddHours(increment))
        {
            rectangle.Time2 = now.AddHours(increment);
        }
    }

    /// <summary>
    /// Ellipse のデフォルト設定
    /// </summary>
    /// <param name="ellipse"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartEllipse ellipse, TimeFrame timeFrame)
    {
        ellipse.Comment ??= timeFrame.ToString();

        // Y1 が上、Y2 が下となるように修正する。
        if (ellipse.Y1 < ellipse.Y2)
        {
            (ellipse.Y1, ellipse.Y2) = (ellipse.Y2, ellipse.Y1);
        }

        // Time1 が左辺、Time2 が右辺となるように修正する。
        if (ellipse.Time1 > ellipse.Time2)
        {
            (ellipse.Time1, ellipse.Time2) = (ellipse.Time2, ellipse.Time1);
        }

        ellipse.Thickness = 1;
        ellipse.LineStyle = LineStyle.Solid;
        ellipse.IsFilled = true;
    }

    /// <summary>
    /// Ellipse の中心をラインが通るように調整する
    /// </summary>
    /// <param name="ellipse"></param>
    /// <param name="hLines"></param>
    public static void AdjustEllipseSize(this ChartEllipse ellipse, ChartHorizontalLine[] hLines)
    {
        // TODO : 複数ある場合の処理
        if (!hLines.Any() || hLines.Count() > 1)
        {
            return;
        }

        var lineY = hLines.Single().Y;
        var diffY1 = Math.Abs(lineY - ellipse.Y1);
        var diffY2 = Math.Abs(lineY - ellipse.Y2);
        var average = (diffY1 + diffY2) / 2;
        ellipse.Y1 = lineY + average;
        ellipse.Y2 = lineY - average;
    }

    /// <summary>
    /// Triangle のデフォルト設定
    /// </summary>
    /// <param name="triangle"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartTriangle triangle, TimeFrame timeFrame)
    {
        triangle.Comment ??= timeFrame.ToString();

        triangle.Thickness = 6;
        triangle.LineStyle = LineStyle.Solid;
        triangle.IsFilled = true;

        // 底辺の水平化
        // triangle.Y3 = triangle.Y2;
    }

    /// <summary>
    /// Text のデフォルト設定
    /// </summary>
    /// <param name="text"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartText text, TimeFrame timeFrame)
    {
        text.Comment ??= timeFrame.ToString();
    }

    /// <summary>
    /// StaticText のデフォルト設定
    /// </summary>
    /// <param name="staticText"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartStaticText staticText, TimeFrame timeFrame)
    {
        staticText.Comment ??= timeFrame.ToString();
    }

    /// <summary>
    /// Icon のデフォルト設定
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="timeFrame"></param>
    public static void SetDefault(this ChartIcon icon, TimeFrame timeFrame)
    {
        icon.Comment ??= timeFrame.ToString();
    }
}