using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class ImageFilePreviewHandler : FilePreviewHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        try
        {
            using var stream = fileInfo.OpenReadNoLock();

            BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var image = new Image();

        using (image.Init())
        {
            image.Stretch = Stretch.Uniform;
            image.StretchDirection = StretchDirection.Both;
        }

        return new HandlerResult { Viewer = image };
    }
}
