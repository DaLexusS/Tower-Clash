using UnityEngine;

public static class Util
{
    /// <summary>
    /// Converts seconds into a MM:SS format (e.g., 92 -> "01:32").
    /// </summary>
    public static string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// Formats an integer with thousands separators (e.g., 10000 -> "10,000").
    /// </summary>
    public static string FormatWithCommas(int number)
    {
        return number.ToString("N0");
    }

    /// <summary>
    /// Returns true or false chance roll.
    /// </summary>
    public static bool RollChance(int percent)
    {
        return Random.Range(0, 100) < percent;
    }
}