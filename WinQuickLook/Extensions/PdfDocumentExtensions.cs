using System;
using System.Windows;

using PdfiumViewer;

namespace WinQuickLook.Extensions
{
    internal static class PdfDocumentExtensions
    {
        public static Size GetPageSize(this IPdfDocument pdfDocument)
        {
            const int maxWidthOrHeight = 1200;

            var pageWidth = pdfDocument.PageSizes[0].Width;
            var pageHeight = pdfDocument.PageSizes[0].Height;

            var scaleFactor = maxWidthOrHeight / Math.Max(pageWidth, pageHeight);

            return new Size(Math.Round(scaleFactor * pageWidth), Math.Round(scaleFactor * pageHeight));
        }
    }
}
