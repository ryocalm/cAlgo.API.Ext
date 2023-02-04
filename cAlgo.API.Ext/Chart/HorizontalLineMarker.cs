using System.Globalization;

namespace cAlgo.API.Ext.Chart;

public class HorizontalLineMarker
{
    public ChartEllipse Ellipse;
    public double MidY;
    public DateTime MidTime;
    public string Name;

    public HorizontalLineMarker(ChartEllipse ellipse, int symbolDigits)
    {
        Ellipse = ellipse;
        MidY = Math.Round((ellipse.Y1 + ellipse.Y2) / 2.0, symbolDigits);

        var timeDiff = ellipse.Time2 - ellipse.Time1;
        MidTime = ellipse.Time1.AddTicks(timeDiff.Ticks / 2);
        var midTimeDate = MidTime.ToString("MM/dd");
        Name = $"marker@{MidY.ToString(CultureInfo.CurrentCulture)}({midTimeDate})";
    }
}