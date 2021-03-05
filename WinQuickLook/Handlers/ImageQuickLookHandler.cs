using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ImageQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            try
            {
                using var stream = fileInfo.OpenRead();

                BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var (bitmap, originalSize) = GetImage(fileInfo);

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

            return (image, requestSize, FormatMetadata(originalSize, fileInfo));
        }

        private static string FormatMetadata(Size size, FileInfo fileInfo)
        {
            return $"{size.Width}x{size.Height} - {WinExplorerHelper.GetSizeFormat(fileInfo.Length)}";
        }

        private static (BitmapSource, Size) GetImage(FileInfo fileInfo)
        {
            var (scaledSize, originalSize) = GetScaledImageSize(fileInfo, 1200);

            var bitmap = new BitmapImage();

            using var stream = fileInfo.OpenRead();

            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = (int)scaledSize.Width;
            bitmap.DecodePixelHeight = (int)scaledSize.Height;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            bitmap.Freeze();

            return (bitmap, originalSize);
        }

        private static (Size, Size) GetScaledImageSize(FileInfo fileInfo, int maxSize)
        {
            if (!TryGetImageSize(fileInfo, out var originalSize))
            {
                using var stream = fileInfo.OpenRead();

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

        private static bool TryGetImageSize(FileInfo fileInfo, out Size size)
        {
            try
            {
                using var tag = TagLib.File.Create(fileInfo.FullName);

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
