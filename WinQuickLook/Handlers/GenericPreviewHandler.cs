using System.IO;
using System.Windows;

using WinQuickLook.Controls;

namespace WinQuickLook.Handlers
{
    public class GenericPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            return true;
        }

        public override (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var requestSize = new Size(500, 280);

            var fileViewer = new GeneficFileViewer();

            fileViewer.BeginInit();
            fileViewer.Thumbnail = GetThumbnail(fileName);

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
