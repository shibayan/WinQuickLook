﻿using System.IO;
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

        public FrameworkElement GetElement(string fileName)
        {
            var pdfViewer = new PdfViewer();

            var document = PdfDocument.Load(new MemoryStream(File.ReadAllBytes(fileName)));

            pdfViewer.Document = document;

            var windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.BeginInit();
            windowsFormsHost.Child = pdfViewer;
            windowsFormsHost.Width = document.PageSizes[0].Width * 1.2;
            windowsFormsHost.Height = document.PageSizes[0].Height * 1.2;
            windowsFormsHost.EndInit();

            return windowsFormsHost;
        }
    }
}
