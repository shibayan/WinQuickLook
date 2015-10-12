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

            // 最大サイズを計算
            var maxContentWidth = SystemParameters.WorkArea.Width - 100;
            var maxContentHeight = SystemParameters.WorkArea.Height - 100;

            // 画像 -> テキスト -> プレビューハンドラ -> サムネイル表示の順で調べる
            if (BitmapDecoder.CanDecode(selectedItem))
            {
                var image = BitmapDecoder.GetImage(selectedItem);

                var scaleFactor = 1.0;
                
                if (maxContentWidth < image.PixelWidth || maxContentHeight < image.PixelHeight)
                {
                    var subWidth = image.PixelWidth - maxContentWidth;
                    var subHeight = image.PixelHeight - maxContentHeight;

                    scaleFactor = subWidth > subHeight ? maxContentWidth / image.PixelWidth : maxContentHeight / image.PixelHeight;
                }

                _quickLookWindow.Image = image;
                _quickLookWindow.ContentWidth = Math.Min(image.PixelWidth, image.PixelWidth * scaleFactor);
                _quickLookWindow.ContentHeight = Math.Min(image.PixelHeight, image.PixelHeight * scaleFactor);
            }
            else if (PreviewHandlerHost.GetPreviewHandlerCLSID(selectedItem) != Guid.Empty)
            {
                _quickLookWindow.ContentWidth = maxContentWidth / 1.5;
                _quickLookWindow.ContentHeight = maxContentHeight / 1.5;

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
