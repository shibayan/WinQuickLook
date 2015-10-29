using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinQuickLook.Handlers
{
    public class ImagePreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return ((IList)_supportFormats).Contains(extension);
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var bitmap = GetImage(fileName);

            var maxWidth = (SystemParameters.WorkArea.Width - 100) / 2;
            var maxHeight = (SystemParameters.WorkArea.Height - 100) / 2;

            var scaleFactor = 1.0;

            if (maxWidth < bitmap.PixelWidth || maxHeight < bitmap.PixelHeight)
            {
                var subWidth = bitmap.PixelWidth - maxWidth;
                var subHeight = bitmap.PixelHeight - maxHeight;

                scaleFactor = subWidth > subHeight ? maxWidth / bitmap.PixelWidth : maxHeight / bitmap.PixelHeight;
            }

            var image = new Image();

            image.BeginInit();
            image.Stretch = Stretch.Uniform;
            image.StretchDirection = StretchDirection.DownOnly;
            image.Source = bitmap;
            image.Width = Math.Min(bitmap.PixelWidth, (int)(bitmap.PixelWidth * scaleFactor));
            image.Height = Math.Min(bitmap.PixelHeight, (int)(bitmap.PixelHeight * scaleFactor));
            image.EndInit();

            return image;
        }

        private static readonly string[] _supportFormats =
        {
            ".jpeg", ".jpe", ".jpg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".ico"
        };

        private static BitmapSource GetImage(string fileName)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.UriSource = new Uri(fileName, UriKind.Absolute);
            bitmap.EndInit();

            bitmap.Freeze();

            int stride = bitmap.PixelWidth * 4;
            var pixels = new byte[stride * bitmap.PixelHeight];

            bitmap.CopyPixels(pixels, stride, 0);

            return BitmapSource.Create(bitmap.PixelWidth, bitmap.PixelHeight, 96.0, 96.0, bitmap.Format, bitmap.Palette, pixels, stride);
        }
    }
}
