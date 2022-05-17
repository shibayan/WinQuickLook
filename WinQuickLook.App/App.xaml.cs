using System.Threading;
using System.Windows;

using Windows.Win32;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using WinQuickLook.Extensions;
using WinQuickLook.Handlers;
using WinQuickLook.Messaging;
using WinQuickLook.Shell;

namespace WinQuickLook.App;

public partial class App : Application
{
    public App()
    {
        _serviceProvider = ConfigureService();

        _keyboardHook = _serviceProvider.GetRequiredService<KeyboardHook>();
        _mouseHook = _serviceProvider.GetRequiredService<MouseHook>();

        _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    }

    private readonly Mutex _mutex = new(false, AppParameters.Title);

    private readonly KeyboardHook _keyboardHook;
    private readonly MouseHook _mouseHook;

    private readonly MainWindow _mainWindow;

    private readonly ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!_mutex.WaitOne(0, false))
        {
            Shutdown();

            return;
        }

        PInvoke.MFStartup(PInvoke.MF_VERSION, 1);

        _keyboardHook.PerformSpaceKey = () =>
        {
            var fileSystemInfo = new ShellExplorer().GetSelectedItem();

            if (fileSystemInfo is not null)
            {
                _mainWindow.StartPreview(fileSystemInfo);
                _mainWindow.Show();
            }
        };

        _keyboardHook.Start();

#if !DEBUG
        _mouseHook.Start();
#endif

        _mainWindow.Preload();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();

        PInvoke.MFShutdown();

        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }

    private ServiceProvider ConfigureService()
    {
        var services = new ServiceCollection();

        services.AddSingleton(Dispatcher);

        services.TryAddEnumerable(new[]
        {
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, CodeFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, GenericDirectoryPreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, GenericFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, HtmlFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, ImageFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, MediaFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, PdfFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, ShellFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, SvgFilePreviewHandler>(),
            ServiceDescriptor.Singleton<IFileSystemPreviewHandler, TextFilePreviewHandler>()
        });

        services.AddSingleton<AssociationResolver>();

        services.AddSingleton<KeyboardHook>();
        services.AddSingleton<MouseHook>();

        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }
}
