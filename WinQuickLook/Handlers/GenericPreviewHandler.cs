using System.IO;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class GenericPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            return true;
        }

        public (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var requestSize = new Size(500, 280);

            var fileViewer = new GeneficFileViewer();

            fileViewer.BeginInit();
            fileViewer.Thumbnail = ImagingHelper.GetThumbnail(fileName);

            if (File.Exists(fileName))
            {
                fileViewer.FileInfo = new FileInfo(fileName);
            }
            else
            {
                fileViewer.FileInfo = new DirectoryInfo(fileName);
            }

            fileViewer.EndInit();

            return (fileViewer, requestSize);
        }
    }
}
