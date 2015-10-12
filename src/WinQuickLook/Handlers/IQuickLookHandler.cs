using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookHandler
    {
        bool CanOpen(string fileName);
        UIElement GetElement(string fileName);
    }
}
