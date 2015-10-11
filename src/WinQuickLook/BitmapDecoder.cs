using System;
using System.Collections;
using System.IO;
using System.Windows.Media.Imaging;

namespace WinQuickLook
{
    public class BitmapDecoder
    {
        public static BitmapImage GetImage(string fileName)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(fileName);
            bitmap.EndInit();

            return bitmap;
        }

        public static bool CanDecode(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return ((IList)_supportFormats).Contains(extension);
        }

        private static readonly string[] _supportFormats = {
            ".jpeg", ".jpe", ".jpg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".ico"
        };
    }
}
