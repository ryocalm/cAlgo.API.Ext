namespace cAlgo.API.Ext;

public static class TimeFrameExtensions
{
    /// <summary>
    /// TimeFrame の省略記法の文字列を返す。
    /// </summary>
    /// <param name="timeFrame">TimeFrame</param>
    /// <returns></returns>
    public static string ToShorthand(this TimeFrame timeFrame)
    {
        if (timeFrame == TimeFrame.Minute)
        {
            return "M1";
        }

        if (timeFrame == TimeFrame.Minute5)
        {
            return "M5";
        }

        if (timeFrame == TimeFrame.Minute15)
        {
            return "M15";
        }

        if (timeFrame == TimeFrame.Minute30)
        {
            return "M30";
        }

        if (timeFrame == TimeFrame.Hour)
        {
            return "H1";
        }

        if (timeFrame == TimeFrame.Hour4)
        {
            return "H4";
        }

        if (timeFrame == TimeFrame.Daily)
        {
            return "D1";
        }

        if (timeFrame == TimeFrame.Weekly)
        {
            return "W1";
        }

        return timeFrame == TimeFrame.Monthly ? "Mo1" : "another TimeFrame";
    }

    /// <summary>
    /// TimeFrame.ToString() で得られる文字列、あるいは省略記法の文字列から
    /// TimeFrame 型に変換する。
    /// </summary>
    /// <param name="timeFrameString">TimeFrame の文字列</param>
    /// <returns></returns>
    public static TimeFrame ToTimeFrame(this string timeFrameString)
    {
        switch (timeFrameString)
        {
            case "Minute":
            case "M1":
                return TimeFrame.Minute;

            case "Minute5":
            case "M5":
                return TimeFrame.Minute5;

            case "Minute15":
            case "M15":
                return TimeFrame.Minute15;

            case "Minute30":
            case "M30":
                return TimeFrame.Minute30;

            case "Hour":
            case "H1":
                return TimeFrame.Hour;

            case "Hour4":
            case "H4":
                return TimeFrame.Hour4;

            case "Daily":
            case "D1":
                return TimeFrame.Daily;

            case "Weekly":
            case "W1":
                return TimeFrame.Weekly;

            case "Monthly":
            case "Mo1":
                return TimeFrame.Monthly;

            default:
                return TimeFrame.Daily;
        }
    }

    /// <summary>
    /// ある TimeFrame よりも引数 の TimeFrame の方が
    /// 指定した差分 diff だけ長期であれば true を返す。
    /// </summary>
    /// <param name="baseTimeFrame">基準とする TimeFrame</param>
    /// <param name="targetTimeFrame">比較対象となる TimeFrame</param>
    /// <param name="diff">TimeFrame の差分。長期方向は正、短期方向は負の数をとる</param>
    /// <returns></returns>
    /// <example>
    /// base : "Minute30" で 1 段階長期（"Hour"）のみ true としたい場合、
    /// base = 3, target = 4 のみ OK となる。
    /// (target - base) = 1 は OK。2 は NG。
    ///
    /// base : "Minute30" で 1 段階短期（"Minute15"）のみ true としたい場合、
    /// base = 3, target = 2 のみ OK となる。
    /// (target - base) = -1 は OK。-2 は NG。
    /// </example>
    private static bool IsLongerTermThanBaseBy(
        TimeFrame baseTimeFrame,
        TimeFrame targetTimeFrame,
        int diff)
    {
        var timeFrameList = new List<TimeFrame>
        {
            TimeFrame.Minute, // 0
            TimeFrame.Minute5, // 1
            TimeFrame.Minute15, // 2
            TimeFrame.Minute30, // 3
            TimeFrame.Hour, // 4
            TimeFrame.Hour4, // 5
            TimeFrame.Daily, // 6
            TimeFrame.Weekly, // 7
            TimeFrame.Monthly, // 8
        };

        var baseIndex = timeFrameList.IndexOf(baseTimeFrame);
        var targetIndex = timeFrameList.IndexOf(targetTimeFrame);
        var indexDiff = targetIndex - baseIndex;

        // TODO 判定式があっているか？
        return diff == indexDiff;
    }

    /// <summary>
    /// 文字列で表された TimeFrame よりも引数 の TimeFrame の方が
    /// 指定した差分 diff だけ長期であれば true を返す。
    /// </summary>
    /// <param name="baseTimeFrame">基準とする TimeFrame</param>
    /// <param name="targetTimeFrame">比較対象となる TimeFrame</param>
    /// <param name="diff">TimeFrame の差分。長期方向は正、短期方向は負の数をとる</param>
    /// <returns></returns>
    public static bool IsLongerTermBy(
        this TimeFrame baseTimeFrame,
        TimeFrame targetTimeFrame,
        int diff)
    {
        return IsLongerTermThanBaseBy(baseTimeFrame, targetTimeFrame, diff);
    }

    /// <summary>
    /// 文字列で表された TimeFrame よりも引数 の TimeFrame の方が
    /// 指定した差分 diff だけ長期であれば true を返す。
    /// </summary>
    /// <param name="timeFrameString">基準とする TimeFrame の文字列</param>
    /// <param name="targetTimeFrame">比較対象となる TimeFrame</param>
    /// <param name="diff">TimeFrame の差分。長期方向は正、短期方向は負の数をとる</param>
    /// <returns></returns>
    public static bool IsLongerTermBy(
        this string timeFrameString,
        TimeFrame targetTimeFrame,
        int diff)
    {
        var baseTimeFrame = timeFrameString.ToTimeFrame();
        return IsLongerTermThanBaseBy(baseTimeFrame, targetTimeFrame, diff);
    }
}