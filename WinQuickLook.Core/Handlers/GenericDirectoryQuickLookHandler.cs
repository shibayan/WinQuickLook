using System;
using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public class GenericDirectoryQuickLookHandler : DirectoryQuickLookHandler
{
    protected override bool CanOpen(DirectoryInfo directoryInfo) => true;

    protected override (FrameworkElement, Size, string) CreateViewer(DirectoryInfo directoryInfo) => throw new NotImplementedException();
}
