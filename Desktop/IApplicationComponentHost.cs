using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public interface IApplicationComponentHost
    {
        MessageBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
        void Exit();
    }
}
