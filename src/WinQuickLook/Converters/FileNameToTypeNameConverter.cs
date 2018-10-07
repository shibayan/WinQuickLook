using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;

using WinQuickLook.Interop;

namespace WinQuickLook.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FileNameToTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileName = (string)value;

            if (Directory.Exists(fileName))
            {
                return Properties.Resources.FileTypeDirectory;
            }

            var sfi = new SHFILEINFO();

            NativeMethods.SHGetFileInfo(fileName, 0, ref sfi, Marshal.SizeOf(sfi), SHGFI.TYPENAME | SHGFI.USEFILEATTRIBUTES);

            return sfi.szTypeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
