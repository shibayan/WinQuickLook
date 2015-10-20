using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public abstract class PreviewHandlerBase : IQuickLookHandler
    {
        public virtual bool AllowsTransparency => true;
        public abstract bool CanOpen(string fileName);
        public abstract FrameworkElement GetElement(string fileName);

        protected static BitmapSource GetThumbnail(string fileName)
        {
            IShellItem shellItem;

            NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);

            var imageFactory = shellItem.QueryInterface<IShellItemImageFactory>();

            IntPtr bitmap;

            imageFactory.GetImage(new SIZE(256, 256), SIIGBF.RESIZETOFIT, out bitmap);

            var image = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            NativeMethods.DeleteObject(bitmap);

            Marshal.ReleaseComObject(imageFactory);
            Marshal.ReleaseComObject(shellItem);

            return image;
        }
    }
}
