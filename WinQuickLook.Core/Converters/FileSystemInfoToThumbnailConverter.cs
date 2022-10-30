using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using WinQuickLook.Windows;

namespace WinQuickLook.Converters;

[ValueConversion(typeof(FileSystemInfo), typeof(BitmapSource))]
public class FileSystemInfoToThumbnailConverter : IValueConverter
{
    private static readonly ThumbnailImageFactory s_previewImageFactory = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var fileSystemInfo = (FileSystemInfo)value;

        var thumbnailImage = s_previewImageFactory.GetImage(fileSystemInfo);

        return thumbnailImage!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
