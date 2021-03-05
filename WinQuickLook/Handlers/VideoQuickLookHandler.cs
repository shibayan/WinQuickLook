using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class VideoQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            if (!TryGetVideoSize(fileInfo.FullName, out var requestSize))
            {
                requestSize = new Size();
            }

            var videoViewer = new VideoFileViewer();

            videoViewer.BeginInit();
            videoViewer.Source = new Uri(fileInfo.FullName, UriKind.Absolute);
            videoViewer.EndInit();

            return (videoViewer, requestSize, FormatMetadata(requestSize, fileInfo));
        }

        private static string FormatMetadata(Size size, FileInfo fileInfo)
        {
            return $"{size.Width}x{size.Height} - {WinExplorerHelper.GetSizeFormat(fileInfo.Length)}";
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
