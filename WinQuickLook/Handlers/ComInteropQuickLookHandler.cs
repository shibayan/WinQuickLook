using System;
using System.Windows;
using System.Windows.Forms.Integration;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ComInteropQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            return PreviewHandlerHost.GetPreviewHandlerCLSID(fileName) != Guid.Empty;
        }

        public (FrameworkElement, Size, string) GetViewer(string fileName)
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

            return (windowsFormsHost, requestSize, WinExplorerHelper.GetFileSize(fileName));
        }
    }
}
