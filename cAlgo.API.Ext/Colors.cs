namespace cAlgo.API.Ext;

public static class Colors
{
    // common settings
    public static readonly Color Background = Color.Black;
    public static readonly Color Foreground = Color.White;
    public static readonly Color GridLines = Color.FromHex("FF262626");
    public static readonly Color PeriodSeparator = Color.FromHex("DD3F3F3F");
    public static readonly Color TickVolume = Color.Aqua;

    // deal map
    private const string AlphaOfDeal = "80";
    public static readonly Color WinningDeal = Color.FromHex(AlphaOfDeal + "68D0F7");
    public static readonly Color LosingDeal = Color.FromHex(AlphaOfDeal + "FF6666");

    // price line
    private const string AlphaOfPriceLine = "80";
    public static readonly Color AskPriceLine = Color.FromHex(AlphaOfPriceLine + "00BF00");
    public static readonly Color BidPriceLine = Color.FromHex(AlphaOfPriceLine + "F15923");

    // position
    private const string AlphaOfPosition = "CC";
    public static readonly Color Buy = Color.FromHex(AlphaOfPosition + "01AF50");
    public static readonly Color Sell = Color.FromHex(AlphaOfPosition + "FF6666");

    // currencies
    private const string AlphaOfCcy = "CC";
    private static readonly Color Usd = Color.FromHex(AlphaOfCcy + "FFFF66");
    private static readonly Color Gbp = Color.FromHex(AlphaOfCcy + "65FE66");
    private static readonly Color Eur = Color.FromHex(AlphaOfCcy + "FF6666");
    private static readonly Color Jpy = Color.FromHex(AlphaOfCcy + "33C1F3");
    private static readonly Color Aud = Color.FromHex(AlphaOfCcy + "0071C1");
    private static readonly Color Nzd = Color.FromHex(AlphaOfCcy + "FF1493");
    private static readonly Color Cad = Color.FromHex(AlphaOfCcy + "8A2BE2");
    private static readonly Color Chf = Color.FromHex(AlphaOfCcy + "FFFAFA");

    private static readonly Color Xau = Color.FromHex(AlphaOfCcy + "737373");
    private static readonly Color Xag = Color.FromHex(AlphaOfCcy + "d5d5d5");

    /// <summary>
    /// 通貨名に応じて設定すべき Color を返す。
    /// </summary>
    /// <param name="ccy"></param>
    /// <returns></returns>
    public static Color QueryCcyColor(string ccy)
    {
        // Gold
        var defaultColor = Color.FromHex(AlphaOfCcy + "FFD700");
        return ccy switch
        {
            "USD" => Usd,
            "GBP" => Gbp,
            "EUR" => Eur,
            "JPY" => Jpy,
            "AUD" => Aud,
            "NZD" => Nzd,
            "CAD" => Cad,
            "CHF" => Chf,
            "XAU" => Xau,
            "XAG" => Xag,
            _ => defaultColor
        };
    }

    // ベースとなる Color
    private const string SupportColor = "02AFF1";
    private const string ResistanceColor = "FF6666";

    // Line、Rectangle、Triangle の透明度
    private const string AlphaOfLine = "CC";
    private const string AlphaOfZone = "35";
    private const string AlphaOfTriangle = "30";

    // Line
    public static readonly Color SupportLine = Color.FromHex(AlphaOfLine + SupportColor);
    public static readonly Color ResistanceLine = Color.FromHex(AlphaOfLine + ResistanceColor);

    // Rectangle
    public static readonly Color SupportZone = Color.FromHex(AlphaOfZone + SupportColor);
    public static readonly Color ResistanceZone = Color.FromHex(AlphaOfZone + ResistanceColor);

    // Triangle
    public static readonly Color TriangleA = Color.FromHex(AlphaOfTriangle + ResistanceColor);
    public static readonly Color TriangleV = Color.FromHex(AlphaOfTriangle + SupportColor);

    // マーカー用 Ellipse
    public static readonly Color LineMarker = Color.FromHex(AlphaOfZone + "FD24FD");
}