using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using PdfiumViewer;

using WinQuickLook.Extensions;
using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class PdfQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return extension == ".pdf";
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var pdfViewer = new PdfViewer();

            var document = PdfDocument.Load(new MemoryStream(File.ReadAllBytes(fileInfo.FullName)));

            pdfViewer.Document = document;
            pdfViewer.ShowBookmarks = false;
            pdfViewer.ShowToolbar = false;

            var requestSize = document.GetPageSize();

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = pdfViewer;
            windowsFormsHost.EndInit();

            return (windowsFormsHost, requestSize, FormatMetadata(document, fileInfo));
        }

        private static string FormatMetadata(PdfDocument document, FileInfo fileInfo)
        {
            return $"{string.Format(Strings.Resources.PageCountText, document.PageCount)} - {WinExplorerHelper.GetSizeFormat(fileInfo.Length)}";
        }
    }
}
