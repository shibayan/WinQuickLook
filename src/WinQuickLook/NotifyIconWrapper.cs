using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace WinQuickLook
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var linkPath = Path.Combine(directory, "WinQuickLook.lnk");

            toolStripMenuItem2.Checked = File.Exists(linkPath);

            toolStripMenuItem1.Click += (sender, e) => Application.Current.Shutdown();
            toolStripMenuItem2.Click += (sender, e) =>
            {
                if (File.Exists(linkPath))
                {
                    File.Delete(linkPath);
                }
                else
                {
                    WinExplorerHelper.CreateShortcutLink(linkPath);
                }
            };
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
