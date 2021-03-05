using System;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using WinQuickLook.Controls;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class ComInteropQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            return PreviewHandlerHost.GetPreviewHandlerCLSID(fileInfo.FullName) != Guid.Empty;
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
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

            previewHandlerHost.Open(fileInfo.FullName);

            return (windowsFormsHost, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
        }
    }
}
