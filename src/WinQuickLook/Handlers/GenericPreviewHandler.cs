using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinQuickLook.Controls;
using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public class GenericPreviewHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            return true;
        }

        public FrameworkElement GetElement(string fileName)
        {
            var bitmap = GetImage(fileName);

            var viewer = new FileInfoViewer();

            viewer.BeginInit();
            viewer.Width = 500;
            viewer.Height = 280;
            viewer.Image = bitmap;

            if (File.Exists(fileName))
            {
                viewer.FileInfo = new FileInfo(fileName);
            }
            else
            {
                viewer.FileInfo = new DirectoryInfo(fileName);
            }

            viewer.EndInit();

            return viewer;
        }

        public bool AllowsTransparency => true;

        private static BitmapSource GetImage(string fileName)
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
