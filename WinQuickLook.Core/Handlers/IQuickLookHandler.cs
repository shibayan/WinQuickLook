using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers;

public interface IQuickLookHandler
{
    bool CanOpen(FileSystemInfo fileSystemInfo);

    (FrameworkElement, Size, string) CreateViewer(FileSystemInfo fileSystemInfo);
}
