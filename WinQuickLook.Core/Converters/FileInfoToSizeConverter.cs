using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace WinQuickLook.Converters;

[ValueConversion(typeof(FileInfo), typeof(long))]
public class FileInfoToSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FileInfo fileInfo)
        {
            return fileInfo.Length;
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
