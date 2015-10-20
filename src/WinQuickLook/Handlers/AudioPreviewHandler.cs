using System;
using System.Windows;

namespace WinQuickLook.Handlers
{
    public class AudioPreviewHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            throw new NotImplementedException();
        }

        public FrameworkElement GetElement(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool AllowsTransparency => true;
    }
}
