using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WinQuickLook.Controls;

public class PreviewHostControl : HwndHost
{
    protected override HandleRef BuildWindowCore(HandleRef hwndParent) => throw new NotImplementedException();

    protected override void DestroyWindowCore(HandleRef hwnd) => throw new NotImplementedException();
}
