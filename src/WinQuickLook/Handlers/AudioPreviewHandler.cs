using System;
using System.Collections;
using System.IO;
using System.Windows;

using WinQuickLook.Controls;

namespace WinQuickLook.Handlers
{
    public class AudioPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return ((IList)_supportFormats).Contains(extension);
        }

        public override FrameworkElement GetElement(string fileName)
        {
            var audioViewer = new AudioFileViewer();

            audioViewer.BeginInit();
            audioViewer.Width = 250;
            audioViewer.Height = 250;
            audioViewer.Source = new Uri(fileName, UriKind.Absolute);
            audioViewer.Thumbnail = GetThumbnail(fileName);
            audioViewer.EndInit();

            return audioViewer;
        }

        private static readonly string[] _supportFormats =
        {
            ".mp3", ".m4a", ".wav", ".wma"
        };
    }
}
