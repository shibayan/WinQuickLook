using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AnimatedGifQuickLookHandler : IQuickLookHandler
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
            using var file = TagLib.File.Create(fileName);

            var requestSize = new Size
            {
                Width = file.Properties.PhotoWidth,
                Height = file.Properties.PhotoHeight
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

            return (mediaElement, requestSize, FormatMetadata(file, fileName));
        }

        private static string FormatMetadata(TagLib.File file, string fileName)
        {
            return $"{file.Properties.PhotoWidth}x{file.Properties.PhotoHeight} - {WinExplorerHelper.GetFileSize(fileName)}";
        }
    }
}
