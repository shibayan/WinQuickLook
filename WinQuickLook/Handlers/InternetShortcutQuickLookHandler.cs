using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace WinQuickLook.Handlers
{
    public class InternetShortcutQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(string fileName)
        {
            var content = File.ReadAllLines(fileName);

            var urlEntry = content.FirstOrDefault(x => x.StartsWith("URL"));

            if (urlEntry == null)
            {
                return (null, default, null);
            }

            var url = urlEntry.Substring(4);

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
                    Source = new Uri(url)
                };

                return (webView2, requestSize, url);
            }
            catch (EdgeNotFoundException)
            {
                var webBrowser = new WebBrowser
                {
                    Source = new Uri(url)
                };

                return (webBrowser, requestSize, url);
            }
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".url"
        };
    }
}
