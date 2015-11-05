using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IQuickLookPreviewHandler
    {
        bool AllowsTransparency { get; }
        bool CanOpen(string fileName);
        FrameworkElement GetElement(string fileName);
    }
}
