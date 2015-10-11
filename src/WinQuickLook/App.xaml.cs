using System;
using System.IO;
using System.Windows;

namespace WinQuickLook
{
    public partial class App
    {
        private KeyboardHook _keyboard;
        private NotifyIconWrapper _notifyIcon;
        private ShellIntegration _shellIntegration;

        private QuickLookWindow _quickLookWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _notifyIcon = new NotifyIconWrapper();
            _shellIntegration = new ShellIntegration();

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
            if (_quickLookWindow != null)
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

            var selectedItem = _shellIntegration.GetSelectedItem();

            if (selectedItem == null)
            {
                return;
            }

            if (_quickLookWindow != null)
            {
                _quickLookWindow.Close();

                _quickLookWindow = null;
            }

            _quickLookWindow = new QuickLookWindow
            {
                FileInfo = new FileInfo(selectedItem)
            };

            // 画像 -> プレビューハンドラ -> サムネイル表示の順で調べる
            if (BitmapDecoder.CanDecode(selectedItem))
            {
                var image = BitmapDecoder.GetImage(selectedItem);

                _quickLookWindow.Image = image;
                _quickLookWindow.ContentWidth = image.PixelWidth;
                _quickLookWindow.ContentHeight = image.PixelHeight;
            }
            else if (PreviewHandlerHost.GetPreviewHandlerGUID(selectedItem) != Guid.Empty)
            {
                _quickLookWindow.ContentWidth = SystemParameters.WorkArea.Width / 1.5;
                _quickLookWindow.ContentHeight = SystemParameters.WorkArea.Height / 1.5;

                _quickLookWindow.OpenPreview();
            }
            else
            {
                var image = _shellIntegration.GetThumbnail(selectedItem);

                _quickLookWindow.Image = image;
                _quickLookWindow.ContentWidth = image.Width;
                _quickLookWindow.ContentHeight = image.Height;
            }

            _quickLookWindow.Show();
        }
    }
}
