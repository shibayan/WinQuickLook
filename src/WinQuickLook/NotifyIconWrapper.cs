using System.ComponentModel;
using System.Windows;

namespace WinQuickLook
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            toolStripMenuItem1.Click += (sender, e) => Application.Current.Shutdown();
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
