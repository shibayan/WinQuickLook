using System;
using System.ComponentModel;
using System.Windows;

namespace WinQuickLook
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            contextMenuStrip1.Opening += async (sender, e) =>
            {
                var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync("WinQuickLookTask");

                toolStripMenuItem2.Checked = startupTask.State == Windows.ApplicationModel.StartupTaskState.Enabled;
            };

            toolStripMenuItem1.Click += (sender, e) => Application.Current.Shutdown();
            toolStripMenuItem2.Click += async (sender, e) =>
            {
                var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync("WinQuickLookTask");

                if (startupTask.State == Windows.ApplicationModel.StartupTaskState.Enabled)
                {
                    startupTask.Disable();

                    toolStripMenuItem2.Checked = false;
                }
                else
                {
                    var state = await startupTask.RequestEnableAsync();

                    toolStripMenuItem2.Checked = state == Windows.ApplicationModel.StartupTaskState.Enabled;
                }
            };
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public event EventHandler Click
        {
            add => notifyIcon1.Click += value;
            remove => notifyIcon1.Click -= value;
        }
    }
}
