using System;
using System.ComponentModel;
using System.Windows;

using Windows.ApplicationModel;

namespace WinQuickLook
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            InitializeContextMenus();
        }

        private void InitializeContextMenus()
        {
            notifyIcon1.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("Icon.ico", UriKind.Relative)).Stream);

            toolStripMenuItem2.Text = Strings.Resources.AutoStartText;
            toolStripMenuItem1.Text = Strings.Resources.ExitText;

            contextMenuStrip1.Opening += async (sender, e) =>
            {
                var startupTask = await StartupTask.GetAsync("WinQuickLookTask");

                toolStripMenuItem2.Checked = startupTask.State == StartupTaskState.Enabled;
            };

            toolStripMenuItem1.Click += (sender, e) => Application.Current.Shutdown();
            toolStripMenuItem2.Click += async (sender, e) =>
            {
                var startupTask = await StartupTask.GetAsync("WinQuickLookTask");

                if (startupTask.State == StartupTaskState.Enabled)
                {
                    startupTask.Disable();

                    toolStripMenuItem2.Checked = false;
                }
                else
                {
                    var state = await startupTask.RequestEnableAsync();

                    toolStripMenuItem2.Checked = state == StartupTaskState.Enabled;
                }
            };
        }

        public event EventHandler Click
        {
            add => notifyIcon1.Click += value;
            remove => notifyIcon1.Click -= value;
        }
    }
}
