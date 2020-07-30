using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ComInteropPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            return PreviewHandlerHost.GetPreviewHandlerCLSID(fileName) != Guid.Empty;
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
            };

            var previewHandlerHost = new PreviewHandlerHost();

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = previewHandlerHost;
            windowsFormsHost.EndInit();

            previewHandlerHost.Open(fileName);

            return (windowsFormsHost, requestSize, $"{WinExplorerHelper.GetFileSize(fileName)}");
        }
    }
}
