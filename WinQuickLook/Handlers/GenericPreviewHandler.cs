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

        public override FrameworkElement GetElement(string fileName)
        {
            var fileViewer = new GeneficFileViewer();

            fileViewer.BeginInit();
            fileViewer.Width = 500;
            fileViewer.Height = 280;
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

            return fileViewer;
        }
    }
}
