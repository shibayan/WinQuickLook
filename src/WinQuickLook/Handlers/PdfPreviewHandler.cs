using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using PdfiumViewer;

namespace WinQuickLook.Handlers
{
    public class PdfPreviewHandler : IQuickLookPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return extension == ".pdf";
        }

        public FrameworkElement GetElement(string fileName)
        {
            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var pdfViewer = new PdfViewer();

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = pdfViewer;
            windowsFormsHost.Width = maxWidth / 1.5;
            windowsFormsHost.Height = maxHeight / 1.5;
            windowsFormsHost.EndInit();

            pdfViewer.Document = PdfDocument.Load(new MemoryStream(File.ReadAllBytes(fileName)));

            return windowsFormsHost;
        }
    }
}
