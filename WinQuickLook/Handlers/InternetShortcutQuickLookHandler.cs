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
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var content = File.ReadAllLines(fileInfo.FullName);

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
            catch (WebView2RuntimeNotFoundException)
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
