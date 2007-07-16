using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class DesktopException : Exception
    {
        public DesktopException(string message)
            :base(message)
        {

        }
    }
}
