﻿using System.Globalization;
using System.Windows.Data;

namespace Music.Applications.Windows.Converters;

public class StringToUpperConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return s.ToUpper();
        }

        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}