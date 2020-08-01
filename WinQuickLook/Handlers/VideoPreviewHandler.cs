using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class VideoPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var tcs = new TaskCompletionSource<(FrameworkElement, Size, string)>();

            var player = new MediaPlayer();

            player.Open(new Uri(fileName, UriKind.Absolute));

            player.MediaOpened += (sender, e) =>
            {
                var requestSize = new Size
                {
                    Width = player.NaturalVideoWidth,
                    Height = player.NaturalVideoHeight
                };

                var videoViewer = new VideoFileViewer();

                videoViewer.BeginInit();
                videoViewer.Source = new Uri(fileName, UriKind.Absolute);
                videoViewer.EndInit();

                tcs.SetResult((videoViewer, requestSize, $"{player.NaturalVideoWidth}x{player.NaturalVideoHeight} - {WinExplorerHelper.GetFileSize(fileName)}"));

                player.Close();
            };

            return tcs.Task;
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov", ".wmv"
        };
    }
}
