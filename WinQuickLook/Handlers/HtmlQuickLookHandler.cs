using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

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

            try
            {
                CoreWebView2Environment.GetAvailableBrowserVersionString();

                var webView2 = new WebView2
                {
                    Source = new Uri(fileName, UriKind.Absolute)
                };

                return (webView2, requestSize, WinExplorerHelper.GetFileSize(fileName));
            }
            catch (EdgeNotFoundException)
            {
                var webBrowser = new WebBrowser
                {
                    Source = new Uri(fileName, UriKind.Absolute)
                };

                return (webBrowser, requestSize, WinExplorerHelper.GetFileSize(fileName));
            }
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".htm", ".html", ".xhtml"
        };
    }
}
