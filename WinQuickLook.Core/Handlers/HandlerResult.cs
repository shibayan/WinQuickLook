using System.Windows;

namespace WinQuickLook.Handlers;

public class HandlerResult
{
    public FrameworkElement Viewer { get; init; } = null!;

    public Size RequestSize { get; set; } = DefaultRequestSize;

    private static readonly Size DefaultRequestSize = new(1200, 900);
}
