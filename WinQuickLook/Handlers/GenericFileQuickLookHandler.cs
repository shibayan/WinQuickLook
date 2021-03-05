using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class GenericFileQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            return true;
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var requestSize = new Size(572, 290);

            var fileViewer = new GeneficFileViewer();

            fileViewer.BeginInit();
            fileViewer.Thumbnail = ImagingHelper.GetThumbnail(fileInfo.FullName);
            fileViewer.FileInfo = fileInfo;

            fileViewer.EndInit();

            return (fileViewer, requestSize, null);
        }
    }
}
