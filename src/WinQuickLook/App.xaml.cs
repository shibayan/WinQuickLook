using System.IO;
using System.Windows;

namespace WinQuickLook
{
    public partial class App
    {
        private KeyboardHook _keyboard;
        private NotifyIconWrapper _notifyIcon;

        private QuickLookWindow _quickLookWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WebBrowserHelper.SetDocumentMode(11000);

            _notifyIcon = new NotifyIconWrapper();

            _keyboard = new KeyboardHook(() => Current.Dispatcher.InvokeAsync(PerformQuickLook), () => Current.Dispatcher.InvokeAsync(CancelQuickLook));
            _keyboard.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _keyboard.Dispose();
            _notifyIcon.Dispose();
        }

        private void CancelQuickLook()
        {
            if (_quickLookWindow != null && _quickLookWindow.IsActive)
            {
                _quickLookWindow.Close();
                _quickLookWindow = null;
            }
        }

        private void PerformQuickLook()
        {
            if (_quickLookWindow != null && _quickLookWindow.IsActive)
            {
                _quickLookWindow.Close();
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
