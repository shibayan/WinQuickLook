using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

using WinQuickLook.Internal;

namespace WinQuickLook.Converters
{
    [ValueConversion(typeof(FileSystemInfo), typeof(string))]
    public class FileInfoToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return DependencyProperty.UnsetValue;
                case FileInfo fileInfo:
                    return WinExplorerHelper.GetSizeFormat(fileInfo.Length);
                case DirectoryInfo directoryInfo:
                    int count = directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length;

                    return string.Format(Strings.Resources.FileItemCount, count);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
