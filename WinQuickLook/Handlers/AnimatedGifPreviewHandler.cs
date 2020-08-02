using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AnimatedGifPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            if (extension != ".gif")
            {
                return false;
            }

            var bitmap = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            return bitmap.Frames.Count > 1;
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            using var tag = TagLib.File.Create(fileName);

            var requestSize = new Size
            {
                Width = tag.Properties.PhotoWidth,
                Height = tag.Properties.PhotoHeight
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

            return (mediaElement, requestSize, $"{tag.Properties.PhotoWidth}x{tag.Properties.PhotoHeight} - {WinExplorerHelper.GetFileSize(fileName)}");
        }
    }
}
