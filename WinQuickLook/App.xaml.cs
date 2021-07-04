using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Hardcodet.Wpf.TaskbarNotification;

using Windows.ApplicationModel;

using WinQuickLook.Extensions;
using WinQuickLook.Internal;
using WinQuickLook.Interop;
using WinQuickLook.Views;

namespace WinQuickLook
{
    public partial class App
    {
        private Mutex _mutex = new(false, "WinQuickLook");

        private MessageHook _messageHook;
        private TaskbarIcon _notifyIcon;

        private readonly QuickLookWindow _quickLookWindow = new();

        private string _currentItem;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!_mutex.WaitOne(0, false))
            {
                _mutex.Close();
                _mutex = null;

                Current.Shutdown();

                return;
            }

#if !DEBUG
            Microsoft.AppCenter.AppCenter.Start("a49cb0c4-9884-4d72-bf96-ccd0e2c4bbe1", typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));
#endif

            NativeMethods.SetProcessDpiAwarenessContext(Consts.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            NativeMethods.DwmIsCompositionEnabled(out var isDwmEnabled);

            if (!isDwmEnabled)
            {
                MessageBox.Show(Strings.Resources.DwmDisabledErrorMessage, "WinQuickLook");

                Current.Shutdown();

                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            _messageHook = new MessageHook(Current.Dispatcher)
            {
                PerformAction = PerformQuickLook,
                ChangeAction = ChangeQuickLook,
                CancelAction = CancelQuickLook
            };

            _messageHook.Start();

            // Preloading Window
            _quickLookWindow.Preload();

            var startupTask = await StartupTask.GetAsync("WinQuickLookTask");

            ((MenuItem)_notifyIcon.ContextMenu.Items[0]).IsChecked = startupTask.State == StartupTaskState.Enabled;
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
                _currentItem = null;

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

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e) => _quickLookWindow?.Activate();

        private async void AutoStartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var startupTask = await StartupTask.GetAsync("WinQuickLookTask");

            if (startupTask.State == StartupTaskState.Enabled)
            {
                startupTask.Disable();

                ((MenuItem)sender).IsChecked = false;
            }
            else
            {
                var state = await startupTask.RequestEnableAsync();

                ((MenuItem)sender).IsChecked = state == StartupTaskState.Enabled;
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Current.Shutdown();
    }
}
