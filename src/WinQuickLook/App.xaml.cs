using System.Threading;
using System.Windows;

namespace WinQuickLook
{
    public partial class App
    {
        private Mutex _mutex = new Mutex(false, "WinQuickLook");

        private KeyboardHook _keyboardHook;
        private NotifyIconWrapper _notifyIcon;

        private readonly QuickLookWindow _quickLookWindow = new QuickLookWindow();

        private string _currentItem;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!_mutex.WaitOne(0, false))
            {
                _mutex.Close();
                _mutex = null;

                Current.Shutdown();

                return;
            }

            _notifyIcon = new NotifyIconWrapper();
            _notifyIcon.Click += (_, __) => { _quickLookWindow?.Activate(); };

            _keyboardHook = new KeyboardHook(Current.Dispatcher)
            {
                PerformAction = PerformQuickLook,
                ChangeAction = ChangeQuickLook,
                CancelAction = CancelQuickLook
            };

            _keyboardHook.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _keyboardHook?.Dispose();
            _notifyIcon?.Dispose();

            _mutex?.ReleaseMutex();
        }

        private void PerformQuickLook()
        {
            var selectedItem = WinExplorerHelper.GetSelectedItem();

            if (selectedItem == null || selectedItem == _currentItem)
            {
                if (_quickLookWindow.HideIfVisible())
                {
                    _currentItem = null;
                }

                return;
            }

            _currentItem = selectedItem;

            _quickLookWindow.Open(selectedItem);
            _quickLookWindow.Show();
        }

        private void ChangeQuickLook()
        {
            if (!_quickLookWindow.IsVisible)
            {
                return;
            }

            var selectedItem = WinExplorerHelper.GetSelectedItem();

            if (selectedItem == null || selectedItem == _currentItem)
            {
                return;
            }

            _currentItem = selectedItem;

            _quickLookWindow.Open(selectedItem);
            _quickLookWindow.Show();
        }

        private void CancelQuickLook()
        {
            _quickLookWindow.HideIfVisible();
        }
    }
}
