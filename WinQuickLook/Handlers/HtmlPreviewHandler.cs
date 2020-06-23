using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WinQuickLook.Handlers
{
    public class HtmlPreviewHandler : IPreviewHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size) GetViewer(string fileName)
        {
            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
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
