using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class AudioQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var requestSize = new Size(600, 300);

            using var tag = TagLib.File.Create(fileName);

            var audioViewer = new AudioFileViewer();

            audioViewer.BeginInit();
            audioViewer.Source = new Uri(fileName, UriKind.Absolute);
            audioViewer.Thumbnail = ImagingHelper.GetThumbnail(fileName);
            audioViewer.Metadata = tag.Tag;
            audioViewer.EndInit();

            return (audioViewer, requestSize, WinExplorerHelper.GetFileSize(fileName));
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".mp3", ".m4a", ".wav", ".wma"
        };
    }
}
