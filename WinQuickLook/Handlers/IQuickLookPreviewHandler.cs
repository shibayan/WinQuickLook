using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookPreviewHandler
    {
        bool CanOpen(string fileName);
        FrameworkElement GetElement(string fileName);
    }
}
