using System.Collections.Generic;
using System.IO;
using System.Windows;

using WinQuickLook.Extensions;
using WinQuickLook.Handlers;

namespace WinQuickLook.App;

public partial class MainWindow : Window
{
    public MainWindow(IEnumerable<IFileSystemPreviewHandler> previewHandlers)
    {
        InitializeComponent();

        _previewHandlers = previewHandlers;
    }

    private readonly IEnumerable<IFileSystemPreviewHandler> _previewHandlers;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var fileInfo = new FileInfo(@"C:\Users\shibayan\Music\iTunes\iTunes Media\Music\Compilations\beatmania IIDX 13 DistorteD ORIGINAL SOU\2-02 BREAK OUT.m4a");

        new WinQuickLook.Shell.ShellPropertyStore().GetMusicProperties(fileInfo);

        if (_previewHandlers.TryCreateViewer(fileInfo, out var handlerResult))
        {
            Width = handlerResult.RequestSize.Width;
            Height = handlerResult.RequestSize.Height + AppParameters.CaptionHeight;

            contentPresenter.Content = handlerResult.Viewer;
        }
    }
}
