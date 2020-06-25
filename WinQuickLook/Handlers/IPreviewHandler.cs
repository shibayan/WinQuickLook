using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IPreviewHandler
    {
        bool CanOpen(string fileName);

        (FrameworkElement, Size, string) GetViewer(string fileName);
    }
}
