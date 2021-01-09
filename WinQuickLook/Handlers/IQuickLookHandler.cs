using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookHandler
    {
        bool CanOpen(string fileName);

        (FrameworkElement, Size, string) GetViewer(string fileName);
    }
}
