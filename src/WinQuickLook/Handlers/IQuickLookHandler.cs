using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookHandler
    {
        bool AllowsTransparency { get; }
        bool CanOpen(string fileName);
        FrameworkElement GetElement(string fileName);
    }
}
