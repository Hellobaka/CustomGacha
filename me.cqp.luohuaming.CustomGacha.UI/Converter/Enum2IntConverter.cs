using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.Converter
{
    [ValueConversion(typeof(OrderOptional), typeof(int))]
    class OrderOptional2IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return (OrderOptional)value;
        }
    }
    [ValueConversion(typeof(DrawOrder2IntConverter), typeof(int))]
    class DrawOrder2IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return (DrawOrder2IntConverter)value;
        }
    }

}
