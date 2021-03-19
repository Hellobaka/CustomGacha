using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace me.cqp.luohuaming.CustomGacha.UI.Converter
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class Bool2IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool c = bool.Parse(value.ToString());
            return c ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return false;
            int c = int.Parse(parameter.ToString());
            return c == 1;
        }
    }
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2VisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Visibility)value)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                default:
                    return false;
            }
        }
    }
}
