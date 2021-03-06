﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Interop;

namespace WinQuickLook.Internal
{
    internal class ImagingHelper
    {
        public static ImageSource GetThumbnail(string fileName)
        {
            NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, typeof(IShellItem).GUID, out var shellItem);

            var imageFactory = shellItem?.QueryInterface<IShellItemImageFactory>();

            if (imageFactory == null)
            {
                return null;
            }

            imageFactory.GetImage(new SIZE(256, 256), SIIGBF.RESIZETOFIT, out var bitmap);

            if (bitmap == IntPtr.Zero)
            {
                return null;
            }

            var image = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            NativeMethods.DeleteObject(bitmap);

            Marshal.FinalReleaseComObject(imageFactory);
            Marshal.FinalReleaseComObject(shellItem);

            return image;
        }
    }
}
