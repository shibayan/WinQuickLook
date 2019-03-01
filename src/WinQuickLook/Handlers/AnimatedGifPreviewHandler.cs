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

        public override FrameworkElement GetElement(string fileName)
        {
            var bitmap = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            var mediaElement = new MediaElement();

            mediaElement.BeginInit();
            mediaElement.Source = new Uri(fileName, UriKind.Absolute);
            mediaElement.LoadedBehavior = MediaState.Play;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.Width = bitmap.Frames[0].PixelWidth;
            mediaElement.Height = bitmap.Frames[0].PixelHeight;
            mediaElement.MediaOpened += (_, __) => mediaElement.Play();
            mediaElement.MediaEnded += (_, __) =>
            {
                mediaElement.Position = TimeSpan.FromMilliseconds(1);
                mediaElement.Play();
            };
            mediaElement.EndInit();

            return mediaElement;
        }
    }
}
