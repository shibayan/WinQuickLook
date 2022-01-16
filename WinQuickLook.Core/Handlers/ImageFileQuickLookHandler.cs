using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WinQuickLook.Handlers;

public class ImageFileQuickLookHandler : FileQuickLookHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        try
        {
            using var stream = fileInfo.OpenRead();

            BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
