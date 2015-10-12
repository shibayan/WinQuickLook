using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public class ShellPreviewHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            return true;
        }

        public UIElement GetElement(string fileName)
        {
            var bitmap = GetImage(fileName);

            var image = new Image();

            image.BeginInit();
            image.Stretch = Stretch.Uniform;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Source = bitmap;
            image.Width = bitmap.PixelWidth;
            image.Height = bitmap.PixelHeight;
            image.EndInit();

            return image;
        }

        private BitmapSource GetImage(string fileName)
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
