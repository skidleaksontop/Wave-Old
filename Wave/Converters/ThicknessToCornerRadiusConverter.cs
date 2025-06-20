using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Wave.Converters
{

    
    public class ThicknessToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)value;
            return new CornerRadius(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius cornerRadius = (CornerRadius)value;
            return new Thickness(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
        }
    }
}
