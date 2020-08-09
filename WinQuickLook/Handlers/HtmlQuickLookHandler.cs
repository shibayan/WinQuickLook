using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class HtmlQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public async Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName)
        {
            var requestSize = new Size
            {
                Width = 1200,
                Height = 900
            };

            var webBrowser = new WebBrowser();

            webBrowser.Navigate(new Uri(fileName, UriKind.Absolute));

            return (webBrowser, requestSize, WinExplorerHelper.GetFileSize(fileName));
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".htm", ".html", ".xhtml"
        };
    }
}
