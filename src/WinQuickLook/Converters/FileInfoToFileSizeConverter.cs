using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace WinQuickLook.Converters
{
    [ValueConversion(typeof(FileSystemInfo), typeof(string))]
    public class FileInfoToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (value is FileInfo fileInfo)
            {
                var length = fileInfo.Length;

                return WinExplorerHelper.GetSizeFormat(length);
            }

            if (value is DirectoryInfo directoryInfo)
            {
                int count = directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length;

                return $"{count} items";
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
