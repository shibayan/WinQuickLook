using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WinQuickLook.Handlers
{
    public class AnimatedGifPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            if (extension != ".gif")
            {
                return false;
            }

            var bitmap = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            return bitmap.Frames.Count > 1;
        }

        public override (FrameworkElement, Size) GetViewer(string fileName, Size maxSize)
        {
            var bitmap = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            var requestSize = new Size
            {
                Width = bitmap.Frames[0].PixelWidth,
                Height = bitmap.Frames[0].PixelHeight
            };

            var mediaElement = new MediaElement();

            mediaElement.BeginInit();
            mediaElement.Source = new Uri(fileName, UriKind.Absolute);
            mediaElement.LoadedBehavior = MediaState.Play;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.MediaOpened += (_, __) => mediaElement.Play();
            mediaElement.MediaEnded += (_, __) =>
            {
                mediaElement.Position = TimeSpan.FromMilliseconds(1);
                mediaElement.Play();
            };
            mediaElement.EndInit();

            return (mediaElement, requestSize);
        }
    }
}
