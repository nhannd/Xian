using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public interface IApplicationComponentHost
    {
        MessageBoxResult ShowMessageBox(string message, MessageBoxButtons buttons);
        void Exit();
    }
}
