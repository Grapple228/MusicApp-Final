using System.Windows;
using System.Windows.Media;

namespace Music.Applications.Windows.Core;

public static class ThemeManager
{
    private static readonly Dictionary<string, string> DarkThemeColors = new()
    {
        { "BackgroundColor", "#252525" },
        { "HoverDarkColor", "#282828" },
        { "HoverLightColor", "#303030" },
        { "SelectionColor", "#202020" },
        { "EmptyImageColor", "#707070" },
        { "AccentColor", "#5E7D33" },
        { "AlertColor", "#FF5757" },
        { "TextColor", "#707070" },
        { "AccentTextColor", "#BDBDBD" },
        { "HoverTextColor", "MediumAquamarine" },
        { "BlockColor", "#303030" },
        { "ThumbColor", "#D4D4D4" }
    };

    private static readonly Dictionary<string, string> LightThemeColors = new()
    {
        { "BackgroundColor", "#DADADA" },
        { "HoverDarkColor", "#D7D7D7" },
        { "HoverLightColor", "#CFCFCF" },
        { "SelectionColor", "#202020" },
        { "EmptyImageColor", "#8F8F8F" },
        { "AccentColor", "#638C2A" },
        { "AlertColor", "#FF5757" },
        { "TextColor", "#8F8F8F" },
        { "AccentTextColor", "#3B3B3B" },
        { "HoverTextColor", "MediumAquamarine" },
        { "BlockColor", "#CFCFCF" },
        { "ThumbColor", "#A3A3A3" }
    };

    public static void ChangeTheme(bool isDark)
    {
        var app = Application.Current;
        var dictionary = isDark ? DarkThemeColors : LightThemeColors;
        foreach (var (key, value) in dictionary)
            app.Resources[key] = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(value));
    }
}