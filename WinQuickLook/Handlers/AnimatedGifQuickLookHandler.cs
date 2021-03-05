using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AnimatedGifQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            if (extension != ".gif")
            {
                return false;
            }

            using var stream = fileInfo.OpenRead();

            var bitmap = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            return bitmap.Frames.Count > 1;
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            using var file = TagLib.File.Create(fileInfo.FullName);

            var requestSize = new Size
            {
                Width = file.Properties.PhotoWidth,
                Height = file.Properties.PhotoHeight
            };

            var mediaElement = new MediaElement();

            mediaElement.BeginInit();
            mediaElement.Source = new Uri(fileInfo.FullName, UriKind.Absolute);
            mediaElement.LoadedBehavior = MediaState.Play;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.MediaOpened += (_, _) => mediaElement.Play();
            mediaElement.MediaEnded += (_, _) =>
            {
                mediaElement.Position = TimeSpan.FromMilliseconds(1);
                mediaElement.Play();
            };
            mediaElement.EndInit();

            return (mediaElement, requestSize, FormatMetadata(file, fileInfo));
        }

        private static string FormatMetadata(TagLib.File file, FileInfo fileInfo)
        {
            return $"{file.Properties.PhotoWidth}x{file.Properties.PhotoHeight} - {WinExplorerHelper.GetSizeFormat(fileInfo.Length)}";
        }
    }
}
