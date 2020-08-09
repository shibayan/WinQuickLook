using System.IO;
using System.Threading.Tasks;
using System.Windows;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class GenericQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            return true;
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var requestSize = new Size(572, 290);

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

            return (fileViewer, requestSize, null);
        }
    }
}
