using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookHandler
    {
        bool CanOpen(string fileName);
        FrameworkElement GetElement(string fileName);
    }
}
