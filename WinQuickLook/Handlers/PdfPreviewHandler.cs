using System;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;

using PdfiumViewer;

namespace WinQuickLook.Handlers
{
    public class PdfPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return extension == ".pdf";
        }

        public (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var pdfViewer = new PdfViewer();

            var document = PdfDocument.Load(new MemoryStream(File.ReadAllBytes(fileName)));

            pdfViewer.Document = document;

            var maxWidth = (monitorSize.Width - 100) / 1.2;
            var maxHeight = (monitorSize.Height - 100) / 1.2;

            var scaleFactor = 1.0;

            var pageWidth = document.PageSizes[0].Width * 1.2;
            var pageHeight = document.PageSizes[0].Height * 1.2;

            if (maxWidth < pageWidth || maxHeight < pageHeight)
            {
                var subWidth = pageWidth - maxWidth;
                var subHeight = pageHeight - maxHeight;

                scaleFactor = subWidth > subHeight ? maxWidth / pageWidth : maxHeight / pageHeight;
            }

            var requestSize = new Size
            {
                Width = Math.Min(pageWidth, (int)(pageWidth * scaleFactor)),
                Height = Math.Min(pageHeight, (int)(pageHeight * scaleFactor))
            };

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = pdfViewer;
            windowsFormsHost.EndInit();

            return (windowsFormsHost, requestSize);
        }
    }
}
