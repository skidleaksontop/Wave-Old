using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wave.Converters
{
    public class IsIconUniformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Stretch.Fill : Stretch.Uniform;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
