using System.IO;
using System.Windows;

using WinQuickLook.Handlers;

namespace WinQuickLook.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var fileInfo = new FileInfo(@"C:\Users\shibayan\Pictures\kazuakix.jpg");

        var filePreviewHandler = new ImageFilePreviewHandler();

        if (filePreviewHandler.TryCreateViewer(fileInfo, out var handlerResult))
        {
            Width = handlerResult.RequestSize.Width;
            Height = handlerResult.RequestSize.Height + 37;

            contentPresenter.Content = handlerResult.Viewer;
        }
    }
}
