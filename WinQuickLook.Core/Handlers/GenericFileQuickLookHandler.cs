using System;
using System.IO;

namespace WinQuickLook.Handlers;

public class GenericFileQuickLookHandler : FileQuickLookHandler
{
    protected override bool CanOpen(FileInfo fileInfo) => true;

    protected override HandlerResult CreateViewer(FileInfo fileInfo) => throw new NotImplementedException();
}
