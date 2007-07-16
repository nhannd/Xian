using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public interface IApplicationView : IView
    {
        IDesktopWindowView OpenWindow(DesktopWindow window);
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
    }
}
