using System;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using WinQuickLook.Controls;

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

        public (FrameworkElement, Size) GetViewer(string fileName)
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

            return (windowsFormsHost, requestSize);
        }
    }
}
