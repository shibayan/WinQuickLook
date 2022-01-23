using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Shell;

namespace WinQuickLook.Shell;

public class ShellImageFactory
{
    public BitmapSource? GetImage(FileSystemInfo fileSystemInfo)
    {
        if (PInvoke.SHCreateItemFromParsingName(fileSystemInfo.FullName, null, out IShellItem? shellItem).Value < 0)
        {
            return null;
        }

        if (shellItem is null)
        {
            return null;
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (shellItem is not IShellItemImageFactory imageFactory)
        {
            Marshal.ReleaseComObject(shellItem);

            return null;
        }

        var size = new SIZE { cx = 256, cy = 256 };

        if (imageFactory.GetImage(size, SIIGBF.SIIGBF_BIGGERSIZEOK, out var bitmapHandle).Value < 0)
        {
            Marshal.ReleaseComObject(shellItem);

            return null;
        }

        try
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmapHandle.DangerousGetHandle(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        catch
        {
            // ignored
            return null;
        }
        finally
        {
            PInvoke.DeleteObject(new HGDIOBJ(bitmapHandle.DangerousGetHandle()));

            Marshal.ReleaseComObject(shellItem);
        }
    }
}
