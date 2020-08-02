using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            using var tag = TagLib.File.Create(fileName);

            var requestSize = new Size
            {
                Width = tag.Properties.VideoWidth,
                Height = tag.Properties.VideoHeight
            };

            var videoViewer = new VideoFileViewer();

            videoViewer.BeginInit();
            videoViewer.Source = new Uri(fileName, UriKind.Absolute);
            videoViewer.EndInit();

            return (videoViewer, requestSize, $"{tag.Properties.VideoWidth}x{tag.Properties.VideoHeight} - {WinExplorerHelper.GetFileSize(fileName)}");
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov", ".wmv"
        };
    }
}
