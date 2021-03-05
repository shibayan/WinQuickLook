using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class DirectoryQuickLookHandler
    {
        public (FrameworkElement, Size, string) GetViewer(DirectoryInfo directoryInfo)
        {
            var requestSize = new Size(572, 290);

            var fileViewer = new GeneficFileViewer();

            fileViewer.BeginInit();
            fileViewer.Thumbnail = ImagingHelper.GetThumbnail(directoryInfo.FullName);
            fileViewer.FileInfo = directoryInfo;

            fileViewer.EndInit();

            return (fileViewer, requestSize, null);
        }
    }
}
