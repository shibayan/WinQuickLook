using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WinQuickLook.Handlers
{
    public class HtmlPreviewHandler : PreviewHandlerBase
    {
        public override bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public override (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize)
        {
            var maxWidth = monitorSize.Width - 100;
            var maxHeight = monitorSize.Height - 100;

            var requestSize = new Size
            {
                Width = maxWidth / 2,
                Height = maxHeight / 2
            };

            var webBrowser = new WebBrowser();

            webBrowser.Navigate(new Uri(fileName, UriKind.Absolute));

            return (webBrowser, requestSize);
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".htm", ".html", ".xhtml"
        };
    }
}
