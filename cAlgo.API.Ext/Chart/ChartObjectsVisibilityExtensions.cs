namespace cAlgo.API.Ext.Chart;

public static class ChartObjectsVisibilityExtensions
{
    /// <summary>
    /// ChartObject を全て非表示にする。
    /// </summary>
    /// <param name="chart"></param>
    public static void HideChartObjects(this API.Chart chart)
    {
        foreach (var horizontalLine in chart.FindAllObjects<ChartHorizontalLine>())
        {
            horizontalLine.IsHidden = true;
        }

        foreach (var verticalLine in chart.FindAllObjects<ChartVerticalLine>())
        {
            verticalLine.IsHidden = true;
        }

        foreach (var trendLine in chart.FindAllObjects<ChartTrendLine>())
        {
            trendLine.IsHidden = true;
        }

        foreach (var rectangle in chart.FindAllObjects<ChartRectangle>())
        {
            rectangle.IsHidden = true;
        }

        foreach (var ellipse in chart.FindAllObjects<ChartEllipse>())
        {
            ellipse.IsHidden = true;
        }

        foreach (var text in chart.FindAllObjects<ChartText>())
        {
            text.IsHidden = true;
        }

        foreach (var icon in chart.FindAllObjects<ChartIcon>())
        {
            icon.IsHidden = true;
        }
    }

    /// <summary>
    /// ChartHorizontalLine は全ての TimeFrame で表示する？
    /// </summary>
    public static void ShowHorizontalLines(this API.Chart chart)
    {
        var horizontalLines = chart.FindAllObjects<ChartHorizontalLine>();

        if (!horizontalLines.Any())
        {
            return;
        }

        foreach (var horizontalLine in horizontalLines)
        {
            var addedTimeFrame = horizontalLine.Comment.ToTimeFrame();

            if (addedTimeFrame >= chart.TimeFrame)
            {
                horizontalLine.IsHidden = false;
            }

            if (addedTimeFrame.IsLongerTermBy(chart.TimeFrame, 3))
            {
                horizontalLine.IsHidden = false;
            }
        }
    }

    /// <summary>
    /// ChartVerticalLine
    /// EconomicEvents のラインは H1 より短期でのみ表示する
    /// </summary>
    public static void ShowVerticalLines(this API.Chart chart)
    {
        var verticalLines = chart.FindAllObjects<ChartVerticalLine>();

        if (!verticalLines.Any())
        {
            return;
        }

        foreach (var verticalLine in verticalLines)
        {
            if (verticalLine.IsEconomicEventsLine())
            {
                verticalLine.IsHidden = (chart.TimeFrame > TimeFrame.Hour);
            }
        }
    }

    /// <summary>
    /// ChartRectangle
    /// </summary>
    public static void ShowRectangles(this API.Chart chart)
    {
        var rectangles = chart.FindAllObjects<ChartRectangle>();

        if (!rectangles.Any())
        {
            return;
        }

        foreach (var rectangle in rectangles)
        {
            var addedTimeFrame = rectangle.Comment.ToTimeFrame();

            // H4 以上で追加したものは M30 以上でのみ表示する
            if (addedTimeFrame >= TimeFrame.Hour4)
            {
                if (chart.TimeFrame >= TimeFrame.Minute30)
                {
                    rectangle.IsHidden = false;
                }
            }

            // M5 以下で追加したものは H1 以下でのみ表示する
            if (addedTimeFrame > TimeFrame.Minute5)
            {
                continue;
            }

            if (chart.TimeFrame <= TimeFrame.Hour)
            {
                rectangle.IsHidden = false;
            }
        }
    }

    /// <summary>
    /// ChartTrendLine
    /// </summary>
    public static void ShowTrendLines(this API.Chart chart)
    {
        var trendLines = chart.FindAllObjects<ChartTrendLine>();

        if (!trendLines.Any())
        {
            return;
        }

        foreach (var trendLine in trendLines)
        {
            var addedTimeFrame = trendLine.Comment.ToTimeFrame();

            // 水平部分線の場合
            if (trendLine.IsHorizontalPartialLine(chart.Symbol))
            {
                // 追加時の TimeFrame および上 3 段階まで表示する
                if (addedTimeFrame.IsLongerTermBy(chart.TimeFrame, 3))
                {
                    trendLine.IsHidden = false;
                }
            }
            // それ以外の場合は追加時の TimeFrame でのみ表示する
            else
            {
                if (addedTimeFrame == chart.TimeFrame)
                {
                    trendLine.IsHidden = false;
                }
            }
        }
    }

    /// <summary>
    /// ChartEllipse
    /// 追加時の TimeFrame より短期、および 3 段階長期まで表示する
    /// </summary>
    public static void ShowEllipses(this API.Chart chart)
    {
        var ellipses = chart.FindAllObjects<ChartEllipse>();

        if (!ellipses.Any())
        {
            return;
        }

        foreach (var ellipse in ellipses)
        {
            var addedTimeFrame = ellipse.Comment.ToTimeFrame();

            if (chart.TimeFrame <= addedTimeFrame)
            {
                ellipse.IsHidden = false;
            }

            if (addedTimeFrame.IsLongerTermBy(chart.TimeFrame, 3))
            {
                ellipse.IsHidden = false;
            }
        }
    }

    /// <summary>
    /// ChartText
    /// 追加時の TimeFrame と同じ TimeFrame なら表示する
    /// </summary>
    public static void ShowTexts(this API.Chart chart)
    {
        var texts = chart.FindAllObjects<ChartText>();

        if (!texts.Any())
        {
            return;
        }

        foreach (var text in texts)
        {
            var addedTimeFrame = text.Comment.ToTimeFrame();

            if (addedTimeFrame == chart.TimeFrame)
            {
                text.IsHidden = false;
            }
        }
    }

    /// <summary>
    /// ChartIcon
    /// 追加時の TimeFrame と同じ TimeFrame なら表示する
    /// </summary>
    public static void ShowIcons(this API.Chart chart)
    {
        var icons = chart.FindAllObjects<ChartIcon>();

        if (!icons.Any())
        {
            return;
        }

        foreach (var icon in icons)
        {
            var addedTimeFrame = icon.Comment.ToTimeFrame();

            if (addedTimeFrame == chart.TimeFrame)
            {
                icon.IsHidden = false;
            }
        }
    }

    /// <summary>
    /// 現在の TimeFrame で表示するべき ChartObject を表示する。
    /// </summary>
    /// <param name="chart"></param>
    public static void ShowChartObjects(this API.Chart chart)
    {
        chart.ShowHorizontalLines();
        chart.ShowVerticalLines();
        chart.ShowRectangles();
        chart.ShowTrendLines();
        chart.ShowEllipses();
        chart.ShowTexts();
        chart.ShowIcons();
    }
}