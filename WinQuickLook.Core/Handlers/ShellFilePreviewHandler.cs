using System;
using System.IO;

using Windows.Win32;
using Windows.Win32.UI.Shell;

using WinQuickLook.Controls;
using WinQuickLook.Extensions;

namespace WinQuickLook.Handlers;

public class ShellFilePreviewHandler : FilePreviewHandler
{
    protected override bool CanOpen(FileInfo fileInfo)
    {
        var pcchOut = 0u;

        var result = PInvoke.AssocQueryString(0x00000004, ASSOCSTR.ASSOCSTR_SHELLEXTENSION, fileInfo.Extension, typeof(IPreviewHandler).GUID.ToString("B"), Span<char>.Empty, ref pcchOut);

        return result.Value >= 0;
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var previewHost = new PreviewHostControl();

        using (previewHost.Init())
        {
            previewHost.Open(fileInfo);
        }

        return new HandlerResult { Viewer = previewHost };
    }
}
