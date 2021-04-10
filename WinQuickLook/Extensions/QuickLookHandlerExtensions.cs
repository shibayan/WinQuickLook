using System.IO;
using System.Windows;

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
            catch
            {
                return _genericFileHandler.GetViewer(fileInfo);
            }
        }
    }
}
