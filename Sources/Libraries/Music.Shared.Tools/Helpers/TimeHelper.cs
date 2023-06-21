namespace Music.Shared.Tools.Helpers;

public static class TimeHelper
{
    /// <summary>
    /// Преобразование милисекунд в строку
    /// </summary>
    /// <param name="duration">Время в милисекундах</param>
    /// <returns>Строка в формате 'HH:mm:ss'</returns>
    public static string GetTimeString(this int duration)
    {
        var ts = TimeSpan.FromMilliseconds(duration);
        var hours = (int)ts.TotalHours;
        return
            $"{(hours == 0 ? "" : (hours < 9 ? "0" + hours : "" + hours) + ":")}{(ts.Minutes < 10 ? "0" : "")}{ts.Minutes}:{(ts.Seconds < 10 ? "0" : "")}{ts.Seconds}";
    }
}