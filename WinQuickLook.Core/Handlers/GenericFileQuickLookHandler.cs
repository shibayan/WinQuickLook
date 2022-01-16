using System;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class GenericFileQuickLookHandler : FileQuickLookHandler
{
    protected override bool CanOpen(FileInfo fileInfo) => true;

    protected override (FrameworkElement, Size, string) CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
