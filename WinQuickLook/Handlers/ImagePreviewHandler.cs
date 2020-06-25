﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ImagePreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using var stream = File.OpenRead(fileName);

                BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public (FrameworkElement, Size, string) GetViewer(string fileName)
        {
            var bitmap = GetImage(fileName);

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

            return (image, requestSize, $"{bitmap.PixelWidth}x{bitmap.PixelHeight} - {WinExplorerHelper.GetFileSize(fileName)}");
        }

        private static BitmapSource GetImage(string fileName)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.UriSource = new Uri(fileName, UriKind.Absolute);
            bitmap.EndInit();

            bitmap.Freeze();

            if (Math.Abs(bitmap.DpiX - 96.0) < 0.1 && Math.Abs(bitmap.DpiY - 96.0) < 0.1)
            {
                return bitmap;
            }

            int stride = bitmap.PixelWidth * 4;
            var pixels = new byte[stride * bitmap.PixelHeight];

            bitmap.CopyPixels(pixels, stride, 0);

            return BitmapSource.Create(bitmap.PixelWidth, bitmap.PixelHeight, 96.0, 96.0, bitmap.Format, bitmap.Palette, pixels, stride);
        }
    }
}
