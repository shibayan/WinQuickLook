using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinQuickLook.Handlers
{
    public class VideoPreviewHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return ((IList)_supportFormats).Contains(extension);
        }

        public FrameworkElement GetElement(string fileName)
        {
            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var media = new MediaElement();

            media.BeginInit();
            media.Stretch = Stretch.Uniform;
            media.StretchDirection = StretchDirection.DownOnly;
            media.Source = new Uri(fileName, UriKind.Absolute);
            media.Width = maxWidth / 2;
            media.Height = maxHeight / 2;
            media.LoadedBehavior = MediaState.Play;
            media.EndInit();

            return media;
        }

        private static readonly string[] _supportFormats =
        {
            ".mp4", ".m4v", ".mpg", ".mpeg", ".avi", ".mov"
        };
    }
}
