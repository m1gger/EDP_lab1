using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LabWork1
{
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOutlier && isOutlier)
                return new SolidColorBrush(Color.FromRgb(0xFF, 0xE0, 0xE0));
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
