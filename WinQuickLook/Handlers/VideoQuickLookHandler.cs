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
            using var file = TagLib.File.Create(fileName);

            var requestSize = new Size
            {
                Width = file.Properties.VideoWidth,
                Height = file.Properties.VideoHeight
            };

            var videoViewer = new VideoFileViewer();

            videoViewer.BeginInit();
            videoViewer.Source = new Uri(fileName, UriKind.Absolute);
            videoViewer.EndInit();

            return (videoViewer, requestSize, FormatMetadata(file, fileName));
        }

        private static string FormatMetadata(TagLib.File file, string fileName)
        {
            return $"{file.Properties.VideoWidth}x{file.Properties.VideoHeight} - {WinExplorerHelper.GetFileSize(fileName)}";
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov", ".wmv"
        };
    }
}
