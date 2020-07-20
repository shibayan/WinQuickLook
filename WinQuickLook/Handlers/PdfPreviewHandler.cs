using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using PdfiumViewer;

using WinQuickLook.Extensions;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class PdfPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return extension == ".pdf";
        }

        public (FrameworkElement, Size, string) GetViewer(string fileName)
        {
            var pdfViewer = new PdfViewer();

            var document = PdfDocument.Load(new MemoryStream(File.ReadAllBytes(fileName)));

            pdfViewer.Document = document;

            var requestSize = document.GetPageSize();

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = pdfViewer;
            windowsFormsHost.EndInit();

            return (windowsFormsHost, requestSize, $"{string.Format(Strings.Resources.PageCountText, document.PageCount)} - {WinExplorerHelper.GetFileSize(fileName)}");
        }
    }
}
