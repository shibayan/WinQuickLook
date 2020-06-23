using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WinQuickLook.Controls;

namespace WinQuickLook.Handlers
{
    public class VideoPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size) GetViewer(string fileName)
        {
            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
            };

            var videoViewer = new VideoFileViewer();

            videoViewer.BeginInit();
            videoViewer.Source = new Uri(fileName, UriKind.Absolute);
            videoViewer.EndInit();

            return (videoViewer, requestSize);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov", ".wmv"
        };
    }
}
