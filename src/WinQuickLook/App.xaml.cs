using System.IO;
using System.Threading;
using System.Windows;

namespace WinQuickLook
{
    public partial class App
    {
        private readonly Mutex _mutex = new Mutex(false, "WinQuickLook");

        private KeyboardHook _keyboardHook;
        private NotifyIconWrapper _notifyIcon;

        private QuickLookWindow _quickLookWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!_mutex.WaitOne(0, false))
            {
                Current.Shutdown();

                return;
            }

            WebBrowserHelper.SetDocumentMode(11000);

            _notifyIcon = new NotifyIconWrapper();
            _notifyIcon.Click += (_, __) => { _quickLookWindow?.Activate(); };

            _keyboardHook = new KeyboardHook(() => Current.Dispatcher.InvokeAsync(PerformQuickLook), () => Current.Dispatcher.InvokeAsync(CancelQuickLook));
            _keyboardHook.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _keyboardHook?.Dispose();
            _notifyIcon?.Dispose();

            _mutex.ReleaseMutex();
        }

        private void CancelQuickLook()
        {
            if (_quickLookWindow?.CloseIfActive() ?? false)
            {
                _quickLookWindow = null;
            }
        }

        private void PerformQuickLook()
        {
            if (_quickLookWindow?.CloseIfActive() ?? false)
            {
                _quickLookWindow = null;

                return;
            }

            var selectedItem = WinExplorerHelper.GetSelectedItem();

            if (selectedItem == null || (!File.Exists(selectedItem) && !Directory.Exists(selectedItem)))
            {
                return;
            }

            _quickLookWindow?.Close();
            _quickLookWindow = null;

            _quickLookWindow = new QuickLookWindow();
            _quickLookWindow.Open(selectedItem);

            _quickLookWindow.Show();
        }
    }
}
