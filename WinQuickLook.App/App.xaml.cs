using System.Threading;
using System.Windows;

using Windows.Win32.UI.Input.KeyboardAndMouse;

using WinQuickLook.Extensions;
using WinQuickLook.Messaging;
using WinQuickLook.Shell;

namespace WinQuickLook.App;

public partial class App
{
    public App(MainWindow mainWindow, KeyboardHook keyboardHook, MouseHook mouseHook)
    {
        InitializeComponent();

        _keyboardHook = keyboardHook;
        _mouseHook = mouseHook;

        _mainWindow = mainWindow;
    }

    private readonly Mutex _mutex = new(false, AppParameters.Title);

    private readonly KeyboardHook _keyboardHook;
    private readonly MouseHook _mouseHook;

    private readonly MainWindow _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!_mutex.WaitOne(0, false))
        {
            Shutdown();

            return;
        }

        _keyboardHook.PerformKeyDown = vkCode =>
        {
            if (vkCode == VIRTUAL_KEY.VK_SPACE)
            {
                Dispatcher.InvokeAsync(PerformPreview);
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
        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }

    private void PerformPreview()
    {
        var fileSystemInfo = new ShellExplorer().GetSelectedItem();

        if (fileSystemInfo is not null)
        {
            _mainWindow.StartPreview(fileSystemInfo);
            _mainWindow.Show();
        }
    }
}
