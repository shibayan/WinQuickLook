using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.PropertiesSystem;

namespace WinQuickLook.Controls;

public class PreviewHostControl : HwndHost
{
    protected override HandleRef BuildWindowCore(HandleRef hwndParent) => throw new NotImplementedException();

    protected override void DestroyWindowCore(HandleRef hwnd) => throw new NotImplementedException();

    private IPreviewHandler? _previewHandler;

    public bool Open(FileInfo fileInfo)
    {
        if (!TryGetPreviewHandlerCLSID(fileInfo, out var clsid))
        {
            return false;
        }

        if (!TryCreatePreviewHandler(clsid, out var previewHandler))
        {
            return false;
        }

        if (!TryInitializePreviewHandler(previewHandler, fileInfo))
        {
            UnloadPreviewHandler(previewHandler);

            return false;
        }

        var rect = new RECT { left = 0, top = 0, right = (int)ActualWidth, bottom = (int)ActualHeight };

        previewHandler.SetWindow(new HWND(Handle.ToInt32()), rect);
        previewHandler.DoPreview();

        if (_previewHandler is not null)
        {
            UnloadPreviewHandler(_previewHandler);
        }

        _previewHandler = previewHandler;

        return true;
    }

    private static unsafe bool TryGetPreviewHandlerCLSID(FileInfo fileInfo, out Guid clsid)
    {
        var pcchOut = 40u;
        var pszOut = stackalloc char[(int)pcchOut];

        var result = PInvoke.AssocQueryString(0x00000004, ASSOCSTR.ASSOCSTR_SHELLEXTENSION, fileInfo.Extension, typeof(IPreviewHandler).GUID.ToString("B"), new PWSTR(pszOut), ref pcchOut);

        if (result.Value < 0)
        {
            clsid = Guid.Empty;

            return false;
        }

        clsid = Guid.Parse(new ReadOnlySpan<char>(pszOut, (int)pcchOut));

        return true;
    }

    private static bool TryCreatePreviewHandler(Guid clsid, [NotNullWhen(true)] out IPreviewHandler? previewHandler)
    {
        var comType = Type.GetTypeFromCLSID(clsid);

        if (comType is null)
        {
            previewHandler = null;

            return false;
        }

        previewHandler = (IPreviewHandler?)Activator.CreateInstance(comType);

        return previewHandler is not null;
    }

    private static unsafe bool TryInitializePreviewHandler(IPreviewHandler previewHandler, FileInfo fileInfo)
    {
        switch (previewHandler)
        {
            case IInitializeWithFile initializeWithFile:
                {
                    var result = initializeWithFile.Initialize(fileInfo.FullName, 0);

                    return result.Value >= 0;
                }
            case IInitializeWithItem initializeWithItem:
                {
                    PInvoke.SHCreateItemFromParsingName(fileInfo.FullName, null, typeof(IShellItem).GUID, out var ppv);

                    var shellItem = (IShellItem)Marshal.GetUniqueObjectForIUnknown(new IntPtr(ppv));

                    var result = initializeWithItem.Initialize(shellItem, 0);

                    Marshal.ReleaseComObject(shellItem);

                    return result.Value >= 0;
                }
            case IInitializeWithStream initializeWithStream:
                {
                    var result = initializeWithStream.Initialize(null, 0);

                    return result.Value >= 0;
                }
            default:
                return false;
        }
    }

    private static void UnloadPreviewHandler(IPreviewHandler previewHandler)
    {
        try
        {
            previewHandler.Unload();
        }
        catch
        {
            // ignored
        }
        finally
        {
            Marshal.FinalReleaseComObject(previewHandler);
        }
    }
}
