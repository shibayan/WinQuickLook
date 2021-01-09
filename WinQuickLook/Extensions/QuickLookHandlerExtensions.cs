using System;
using System.Windows;

using Microsoft.AppCenter.Crashes;

using WinQuickLook.Handlers;

namespace WinQuickLook.Extensions
{
    internal static class QuickLookHandlerExtensions
    {
        private static readonly IQuickLookHandler _genericHandler = new GenericQuickLookHandler();

        public static (FrameworkElement, Size, string) GetViewerWithHandleError(this IQuickLookHandler handler, string fileName)
        {
            try
            {
                return (handler ?? _genericHandler).GetViewer(fileName);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return _genericHandler.GetViewer(fileName);
        }
    }
}
