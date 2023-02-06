namespace cAlgo.API.Ext.Chart;

public class ChartObjectsDefaultSetting
{
    private static API.Chart? _chart;
    private static bool _observationMode;

    public ChartObjectsDefaultSetting(API.Chart chart, bool observationMode)
    {
        _chart = chart;
        _observationMode = observationMode;
    }

    private static void UpdateHorizontalLine(ChartObject obj)
    {
        if (obj is not ChartHorizontalLine horizontalLine)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        horizontalLine.SetColor(_chart.Symbol.Ask);
        horizontalLine.SetDefault(_chart.TimeFrame);
    }

    private void UpdateVerticalLine(ChartObject obj)
    {
        if (obj is not ChartVerticalLine verticalLine)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        verticalLine.SetDefault(_chart.TimeFrame);
    }

    private void UpdateTrendLine(ChartObject obj)
    {
        if (obj is not ChartTrendLine trendLine)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        trendLine.SetColor(_chart.Symbol, _chart.Symbol.Ask);
        trendLine.SetDefault(_chart.TimeFrame, _chart.Symbol);
    }

    private void UpdateRectangle(ChartObject obj)
    {
        if (obj is not ChartRectangle rectangle)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        // Y1 が上辺、Y2 が下辺となるように修正する。
        if (rectangle.Y1 < rectangle.Y2)
        {
            (rectangle.Y2, rectangle.Y1) = (rectangle.Y1, rectangle.Y2);
        }

        // Time1 が左辺、Time2 が右辺となるように修正する。
        if (rectangle.Time1 > rectangle.Time2)
        {
            (rectangle.Time2, rectangle.Time1) = (rectangle.Time1, rectangle.Time2);
        }

        // Rectangle の上下にある bar
        var bars = _chart.Bars
            .Where(bar => bar.OpenTime >= rectangle.Time1
                          && bar.OpenTime <= rectangle.Time2)
            .ToArray();

        rectangle.SetColor(bars, _chart.Symbol.Ask, _chart.Symbol.Bid);
        rectangle.SetDefault(_chart.TimeFrame);
    }

    private void UpdateEllipse(ChartObject obj)
    {
        if (obj is not ChartEllipse ellipse)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        ellipse.SetColor();
        ellipse.SetDefault(_chart.TimeFrame);

        // Ellipse を通る HorizontalLine
        var hLines = _chart.FindAllObjects<ChartHorizontalLine>()
            .Where(line => ellipse.Y2 < line.Y && line.Y < ellipse.Y1)
            .ToArray();
        ellipse.AdjustEllipseSize(hLines);
    }

    private void UpdateTriangle(ChartObject obj)
    {
        if (obj is not ChartTriangle triangle)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        triangle.SetColor();
        triangle.SetDefault(_chart.TimeFrame);
    }

    private void UpdateText(ChartObject obj)
    {
        if (obj is not ChartText text)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        text.SetColor();
        text.SetDefault(_chart.TimeFrame);
    }

    private void UpdateStaticText(ChartObject obj)
    {
        if (obj is not ChartStaticText staticText)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        staticText.SetColor();
        staticText.SetDefault(_chart.TimeFrame);
    }

    private void UpdateIcon(ChartObject obj)
    {
        if (obj is not ChartIcon icon)
        {
            return;
        }

        if (_chart == null)
        {
            return;
        }

        icon.SetColor();
        icon.SetDefault(_chart.TimeFrame);
    }

    /// <summary>
    /// ChartObject 追加時に実行する。
    /// 個々の object に対する処理。
    /// </summary>
    /// <param name="obj"></param>
    public void OnAdded(ChartObject obj)
    {
        switch (obj.ObjectType)
        {
            case ChartObjectType.HorizontalLine:
                UpdateHorizontalLine(obj);
                break;

            case ChartObjectType.VerticalLine:
                UpdateVerticalLine(obj);
                break;

            case ChartObjectType.TrendLine:
                UpdateTrendLine(obj);
                break;

            case ChartObjectType.Rectangle:
                UpdateRectangle(obj);
                break;

            case ChartObjectType.Ellipse:
                UpdateEllipse(obj);
                break;

            case ChartObjectType.Triangle:
                UpdateTriangle(obj);
                break;

            case ChartObjectType.Text:
                UpdateText(obj);
                break;

            case ChartObjectType.StaticText:
                UpdateStaticText(obj);
                break;

            case ChartObjectType.Icon:
                UpdateIcon(obj);
                break;

            case ChartObjectType.ArrowLine:
                break;
        }
    }

    /// <summary>
    /// ChartObject 更新時に実行する。
    /// 個々の object に対する処理。
    /// </summary>
    /// <param name="obj"></param>
    public void OnUpdated(ChartObject obj)
    {
        if (_chart == null)
        {
            return;
        }

        switch (obj.ObjectType)
        {
            case ChartObjectType.HorizontalLine:
                if (obj is not ChartHorizontalLine horizontalLine)
                {
                    return;
                }

                if (!_observationMode)
                {
                    horizontalLine.SetColor(_chart.Symbol.Ask);
                }

                horizontalLine.SetDefault(_chart.TimeFrame);
                break;

            case ChartObjectType.VerticalLine:
                break;

            case ChartObjectType.TrendLine:
                if (obj is not ChartTrendLine trendLine)
                {
                    return;
                }

                if (!_observationMode)
                {
                    trendLine.SetColor(_chart.Symbol, _chart.Symbol.Ask);
                }

                trendLine.SetDefault(_chart.TimeFrame, _chart.Symbol);
                break;

            case ChartObjectType.Rectangle:
                UpdateRectangle(obj);
                break;

            case ChartObjectType.Ellipse:
                UpdateEllipse(obj);
                break;

            case ChartObjectType.Triangle:
                UpdateTriangle(obj);
                break;

            case ChartObjectType.Text:
                UpdateText(obj);
                break;

            case ChartObjectType.StaticText:
                UpdateStaticText(obj);
                break;

            case ChartObjectType.Icon:
                UpdateIcon(obj);
                break;

            case ChartObjectType.ArrowLine:
                break;
        }
    }
}