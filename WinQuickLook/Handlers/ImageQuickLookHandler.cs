using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ImageQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            try
            {
                using var stream = File.OpenRead(fileName);

                BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var (bitmap, originalSize) = GetImage(fileName);

            var requestSize = new Size
            {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight
            };

            var image = new Image();

            image.BeginInit();
            image.Stretch = Stretch.Uniform;
            image.StretchDirection = StretchDirection.DownOnly;
            image.Source = bitmap;
            image.EndInit();

            return (image, requestSize, FormatMetadata(originalSize, fileName));
        }

        private static string FormatMetadata(Size size, string fileName)
        {
            return $"{size.Width}x{size.Height} - {WinExplorerHelper.GetFileSize(fileName)}";
        }

        private static (BitmapSource, Size) GetImage(string fileName)
        {
            var (scaledSize, originalSize) = GetScaledImageSize(fileName, 1200);

            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = (int)scaledSize.Width;
            bitmap.DecodePixelHeight = (int)scaledSize.Height;
            bitmap.UriSource = new Uri(fileName, UriKind.Absolute);
            bitmap.EndInit();

            bitmap.Freeze();

            return (bitmap, originalSize);
        }

        private static (Size, Size) GetScaledImageSize(string fileName, int maxSize)
        {
            using var tag = TagLib.File.Create(fileName);

            var width = tag.Properties.PhotoWidth;
            var height = tag.Properties.PhotoHeight;

            var originalSize = new Size(width, height);

            if (width > maxSize || height > maxSize)
            {
                var scaleFactor = (double)maxSize / Math.Max(width, height);

                return (new Size(width * scaleFactor, height * scaleFactor), originalSize);
            }

            return (originalSize, originalSize);
        }
    }
}
