using System;
using System.IO;
using System.Windows;

using Microsoft.AppCenter.Crashes;

using WinQuickLook.Handlers;

namespace WinQuickLook.Extensions
{
    internal static class QuickLookHandlerExtensions
    {
        private static readonly IQuickLookHandler _genericFileHandler = new GenericFileQuickLookHandler();

        public static (FrameworkElement, Size, string) GetViewerWithHandleError(this IQuickLookHandler handler, FileInfo fileInfo)
        {
            try
            {
                return (handler ?? _genericFileHandler).GetViewer(fileInfo);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return _genericFileHandler.GetViewer(fileInfo);
        }
    }
}
