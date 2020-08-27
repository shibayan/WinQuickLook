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
            if (!TryGetImageSize(fileName, out var originalSize))
            {
                using var stream = File.OpenRead(fileName);

                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

                originalSize = new Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
            }

            if (originalSize.Width > maxSize || originalSize.Height > maxSize)
            {
                var scaleFactor = maxSize / Math.Max(originalSize.Width, originalSize.Height);

                return (new Size(originalSize.Width * scaleFactor, originalSize.Height * scaleFactor), originalSize);
            }

            return (originalSize, originalSize);
        }

        private static bool TryGetImageSize(string fileName, out Size size)
        {
            try
            {
                using var tag = TagLib.File.Create(fileName);

                size = new Size(tag.Properties.PhotoWidth, tag.Properties.PhotoHeight);

                return true;
            }
            catch
            {
                size = default;

                return false;
            }
        }
    }
}
