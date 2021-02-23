using System;
using System.Globalization;
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
}
