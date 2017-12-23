using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public abstract class PreviewHandlerBase : IQuickLookPreviewHandler
    {
        public virtual bool AllowsTransparency => true;
        public abstract bool CanOpen(string fileName);
        public abstract FrameworkElement GetElement(string fileName);

        protected static BitmapSource GetThumbnail(string fileName)
        {
            NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out var shellItem);

            var imageFactory = shellItem.QueryInterface<IShellItemImageFactory>();

            imageFactory.GetImage(new SIZE(256, 256), SIIGBF.RESIZETOFIT, out var bitmap);

            var image = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            NativeMethods.DeleteObject(bitmap);

            Marshal.ReleaseComObject(imageFactory);
            Marshal.ReleaseComObject(shellItem);

            return image;
        }
    }
}
