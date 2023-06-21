using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Music.Shared.Tools.Helpers;

namespace Music.Applications.Windows.Converters;

public static class TimeHelper
{
    public static string GetTimeString(this long duration)
    {
        var ts = TimeSpan.FromMilliseconds(duration);
        var hours = (long)ts.TotalHours;
        return $"{(hours == 0 ? "" : (hours < 9 ? "0" + hours : "" + hours) + ":")}{(ts.Minutes < 10 ? "0" : "")}{ts.Minutes}:{(ts.Seconds < 10 ? "0" : "")}{ts.Seconds}";
    }
}

public class TimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (long)value;
        return (val == 0 ? 0 : val / 176).GetTimeString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}