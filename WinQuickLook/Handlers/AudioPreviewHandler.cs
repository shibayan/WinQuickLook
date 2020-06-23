using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AudioPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size) GetViewer(string fileName)
        {
            var requestSize = new Size(300, 300);

            var audioViewer = new AudioFileViewer();

            audioViewer.BeginInit();
            audioViewer.Source = new Uri(fileName, UriKind.Absolute);
            audioViewer.Thumbnail = ImagingHelper.GetThumbnail(fileName);
            audioViewer.EndInit();

            return (audioViewer, requestSize);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp3", ".m4a", ".wav", ".wma"
        };
    }
}
