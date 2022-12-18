using cAlgo.API.Internals;

namespace cAlgo.API.Ext;

public static class SymbolExtensions
{
    public static double GetPip(this Symbol symbol)
    {
        return symbol.TickSize / symbol.PipSize * Math.Pow(10, symbol.Digits);
    }

    public static double ToPips(this Symbol symbol, double price)
    {
        return price * symbol.GetPip();
    }

    /// <summary>
    /// 2 つの price 間の距離を pips で求める
    /// </summary>
    /// <param name="symbol">the Symbol</param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>pips 単位の距離</returns>
    public static double GetDistanceInPips(this Symbol symbol, double from, double to)
    {
        // 小数部 5 桁に四捨五入した値
        var distance = Math.Round(
            value: Math.Abs(from - to),
            digits: 5,
            mode: MidpointRounding.AwayFromZero);

        return Math.Round(
            value: distance / symbol.PipSize,
            digits: symbol.Digits,
            mode: MidpointRounding.AwayFromZero);
    }

    /*
    /// <summary>
    /// 任意の volume を VolumeInUnitStep の倍数となるように調整する。
    /// ただ、NormalizeVolumeInUnits で事足りるか。
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    public static double NormalizeVolume(this Symbol symbol, double volume)
    {
        var result = Math.Floor(volume / symbol.VolumeInUnitsStep) * symbol.VolumeInUnitsStep;
        return result <= symbol.VolumeInUnitsMax ? result : symbol.VolumeInUnitsMax;
    }
    */
}