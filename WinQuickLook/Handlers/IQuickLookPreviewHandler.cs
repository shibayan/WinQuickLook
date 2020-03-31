using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IPreviewHandler
    {
        bool CanOpen(string fileName);
        FrameworkElement GetElement(string fileName);
    }
}
