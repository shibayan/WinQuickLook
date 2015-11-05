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

            var fileInfo = value as FileInfo;

            if (fileInfo != null)
            {
                var length = fileInfo.Length;

                return GetSizeFormat(length);
            }

            var directoryInfo = value as DirectoryInfo;

            if (directoryInfo != null)
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

        private static object GetSizeFormat(long length)
        {
            if (length >= 1024 * 1024 * 1024)
            {
                return $"{length / (double)(1024 * 1024 * 1024):0.##} GB";
            }
            if (length >= 1024 * 1024)
            {
                return $"{length / (double)(1024 * 1024):0.##} MB";
            }
            if (length >= 1024)
            {
                return $"{length / (double)1024:0.##} KB";
            }

            return $"{length} B";
        }
    }
}
