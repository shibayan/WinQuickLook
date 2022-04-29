using System;
using System.Threading;
using System.Windows;

using Windows.Win32;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using WinQuickLook.Handlers;
using WinQuickLook.Shell;

namespace WinQuickLook.App;

public partial class App : Application
{
    public App()
    {
        _serviceProvider = ConfigureService();

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    }

    private readonly Mutex _mutex = new(false, AppParameters.Title);

    private readonly KeyboardHook _keyboardHook = new();
    private readonly MouseHook _mouseHook = new();

    private readonly IServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!_mutex.WaitOne(0, false))
        {
            Shutdown();

            return;
        }

        PInvoke.MFStartup(PInvoke.MF_VERSION, 1);

        _keyboardHook.Start();

#if !DEBUG
        _mouseHook.Start();
#endif

        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        PInvoke.MFShutdown();

        _keyboardHook.Dispose();
        _mouseHook.Dispose();

        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }

    private IServiceProvider ConfigureService()
    {
        var services = new ServiceCollection();

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

        services.AddSingleton<ShellAssociation>();

        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }
}
