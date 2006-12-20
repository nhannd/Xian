using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    public interface IActionView : IView
    {
        void SetAction(IAction action);
    }
}
