using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AudioQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var requestSize = new Size(600, 300);

            using var tag = TagLib.File.Create(fileInfo.FullName);

            var audioViewer = new AudioFileViewer();

            audioViewer.BeginInit();
            audioViewer.Source = new Uri(fileInfo.FullName, UriKind.Absolute);
            audioViewer.Thumbnail = ImagingHelper.GetThumbnail(fileInfo.FullName);
            audioViewer.Metadata = tag.Tag;
            audioViewer.EndInit();

            return (audioViewer, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp3", ".m4a", ".wav", ".wma"
        };
    }
}
