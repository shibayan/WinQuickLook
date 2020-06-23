using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IPreviewHandler
    {
        bool CanOpen(string fileName);

        (FrameworkElement, Size) GetViewer(string fileName, Size monitorSize);
    }
}
