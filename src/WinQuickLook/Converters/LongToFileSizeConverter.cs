using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinQuickLook.Converters
{
    [ValueConversion(typeof(long), typeof(string))]
    public class LongToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            
            var length = (long)value;

            if (length > 1024 * 1024 * 1024)
            {
                return $"{length / (double)(1024 * 1024 * 1024):0.##} GB";
            }
            if (length > 1024 * 1024)
            {
                return $"{length / (double)(1024 * 1024):0.##} MB";
            }
            if (length > 1024)
            {
                return $"{length / (double)1024:0.##} KB";
            }

            return $"{length} B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
