using System;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.AppCenter.Crashes;

using WinQuickLook.Handlers;

namespace WinQuickLook.Extensions
{
    internal static class QuickLookHandlerExtensions
    {
        private static readonly IQuickLookHandler _genericHandler = new GenericQuickLookHandler();

        public static async Task<(FrameworkElement, Size, string)> GetViewerWithErrorAsync(this IQuickLookHandler handler, string fileName)
        {
            try
            {
                return await (handler ?? _genericHandler).GetViewerAsync(fileName);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return await _genericHandler.GetViewerAsync(fileName);
        }
    }
}
