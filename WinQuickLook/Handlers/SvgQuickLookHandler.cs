using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

using WinQuickLook.Internal;

namespace WinQuickLook.Handlers
{
    public class SvgQuickLookHandler : IQuickLookHandler
    {
        public bool CanOpen(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension.ToLower();

            return _supportFormats.Contains(extension);
        }

        public (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo)
        {
            var requestSize = GetScaledImageSize(fileInfo, 1200);

            try
            {
                CoreWebView2Environment.GetAvailableBrowserVersionString();

                var webView2 = new WebView2
                {
                    Source = new Uri(fileInfo.FullName, UriKind.Absolute)
                };

                return (webView2, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
            }
            catch (WebView2RuntimeNotFoundException)
            {
                var webBrowser = new WebBrowser
                {
                    Source = new Uri(fileInfo.FullName, UriKind.Absolute)
                };

                return (webBrowser, requestSize, WinExplorerHelper.GetSizeFormat(fileInfo.Length));
            }
        }

        private static Size GetImageSize(FileInfo fileInfo)
        {
            var document = XDocument.Load(fileInfo.FullName);

            var viewBox = document.Root.Attribute("viewBox");
            var width = document.Root.Attribute("width");
            var height = document.Root.Attribute("height");

            if (viewBox != null)
            {
                var values = viewBox.Value
                                    .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(double.Parse)
                                    .ToArray();

                if (values.Length == 4)
                {
                    return new Size(values[2] - values[0] + 10, values[3] - values[1] + 10);
                }

                if (values.Length == 2)
                {
                    return new Size(values[0] + 10, values[1] + 10);
                }

                return new Size(values[0] + 10, values[0] + 10);
            }

            if (width != null && height != null)
            {
                return new Size(double.Parse(width.Value) + 10, double.Parse(height.Value) + 10);
            }

            return new Size(400, 400);
        }

        private static Size GetScaledImageSize(FileInfo fileInfo, int maxSize)
        {
            var originalSize = GetImageSize(fileInfo);

            if (originalSize.Width > maxSize || originalSize.Height > maxSize)
            {
                var scaleFactor = maxSize / Math.Max(originalSize.Width, originalSize.Height);

                return new Size(originalSize.Width * scaleFactor, originalSize.Height * scaleFactor);
            }

            return originalSize;
        }

        private static readonly IList<string> _supportFormats = new[]
        {
            ".svg"
        };
    }
}
