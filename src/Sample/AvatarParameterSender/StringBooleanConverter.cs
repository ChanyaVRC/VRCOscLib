using System;
using System.Globalization;
using System.Windows.Data;

namespace AvatarParameterSender;
public class StringBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == bool.TrueString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((bool)value).ToString();
    }
}
