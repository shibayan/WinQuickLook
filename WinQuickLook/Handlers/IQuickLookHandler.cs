using System.IO;
using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookHandler
    {
        bool CanOpen(FileInfo fileInfo);

        (FrameworkElement, Size, string) GetViewer(FileInfo fileInfo);
    }
}
