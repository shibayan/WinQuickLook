using System;
using System.IO;

namespace WinQuickLook.Handlers;

public class GenericDirectoryPreviewHandler : DirectoryPreviewHandler
{
    protected override bool CanOpen(DirectoryInfo directoryInfo) => true;

    protected override HandlerResult CreateViewer(DirectoryInfo directoryInfo) => throw new NotImplementedException();
}
