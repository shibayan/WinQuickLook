using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class VideoQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            if (!TryGetVideoSize(fileName, out var requestSize))
            {
                requestSize = new Size();
            }

            var videoViewer = new VideoFileViewer();

            videoViewer.BeginInit();
            videoViewer.Source = new Uri(fileName, UriKind.Absolute);
            videoViewer.EndInit();

            return (videoViewer, requestSize, FormatMetadata(requestSize, fileName));
        }

        private static string FormatMetadata(Size size, string fileName)
        {
            return $"{size.Width}x{size.Height} - {WinExplorerHelper.GetFileSize(fileName)}";
        }

        private static bool TryGetVideoSize(string fileName, out Size size)
        {
            try
            {
                using var tag = TagLib.File.Create(fileName);

                size = new Size(tag.Properties.VideoWidth, tag.Properties.VideoHeight);

                return true;
            }
            catch
            {
                size = default;

                return false;
            }
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov", ".wmv"
        };
    }
}
