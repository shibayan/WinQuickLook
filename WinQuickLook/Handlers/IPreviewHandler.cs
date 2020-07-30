using System.Threading.Tasks;
using System.Windows;

namespace WinQuickLook.Handlers
{
    public interface IPreviewHandler
    {
        bool CanOpen(string fileName);

        Task<(FrameworkElement, Size, string)> GetViewerAsync(string fileName);
    }
}
