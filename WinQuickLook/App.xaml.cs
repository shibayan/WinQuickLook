using System.Text;
using System.Threading;
using System.Windows;

#if !DEBUG
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif

using WinQuickLook.Extensions;
using WinQuickLook.Internal;
using WinQuickLook.Interop;

namespace WinQuickLook
{
    public partial class App
    {
        private Mutex _mutex = new Mutex(false, "WinQuickLook");

        private MessageHook _messageHook;
        private NotifyIconWrapper _notifyIcon;

        private readonly QuickLookWindow _quickLookWindow = new QuickLookWindow();

        private string _currentItem;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            NativeMethods.DwmIsCompositionEnabled(out var isDwmEnabled);

            if (!isDwmEnabled)
            {
                MessageBox.Show(Strings.Resources.DwmDisabledErrorMessage, "WinQuickLook");

                Current.Shutdown();

                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if !DEBUG
            AppCenter.Start("a49cb0c4-9884-4d72-bf96-ccd0e2c4bbe1", typeof(Analytics), typeof(Crashes));
#endif

            if (!_mutex.WaitOne(0, false))
            {
                _mutex.Close();
                _mutex = null;

                Current.Shutdown();

                return;
            }

            NativeMethods.SetProcessDpiAwarenessContext(Consts.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            _notifyIcon = new NotifyIconWrapper();
            _notifyIcon.Click += (_, __) => { _quickLookWindow?.Activate(); };

            _messageHook = new MessageHook(Current.Dispatcher)
            {
                PerformAction = PerformQuickLook,
                ChangeAction = ChangeQuickLook,
                CancelAction = CancelQuickLook
            };

            _messageHook.Start();

            // Preloading Window
            _quickLookWindow.Preload();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _quickLookWindow.Close();

            _messageHook?.Dispose();
            _notifyIcon?.Dispose();

            _mutex?.ReleaseMutex();
        }

        private void PerformQuickLook()
        {
            if (_quickLookWindow.HideIfVisible())
            {
                _currentItem =  null;

                return;
            }

            var selectedItem = WinExplorerHelper.GetSelectedItem();

            if (selectedItem == null)
            {
                return;
            }

            _currentItem = selectedItem;

            _quickLookWindow.Open(selectedItem);
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
        }

        private void CancelQuickLook()
        {
            _quickLookWindow.HideIfVisible();

            _currentItem = null;
        }
    }
}
