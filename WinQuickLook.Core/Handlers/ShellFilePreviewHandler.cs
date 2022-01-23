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
        var riid = typeof(IPreviewHandler).GUID.ToString("B");

        return PInvoke.AssocQueryString(0x00000004, ASSOCSTR.ASSOCSTR_SHELLEXTENSION, fileInfo.Extension, riid, Span<char>.Empty, ref pcchOut).Value >= 0;
    }

    protected override HandlerResult CreateViewer(FileInfo fileInfo)
    {
        var shellFileControl = new ShellFileControl();

        using (shellFileControl.Init())
        {
            shellFileControl.Open(fileInfo);
        }

        return new HandlerResult { Viewer = shellFileControl };
    }
}
