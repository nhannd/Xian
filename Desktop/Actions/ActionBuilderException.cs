using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    public class ActionBuilderException : Exception
    {
        public ActionBuilderException(string message)
            : base(message)
        {
        }
    }
}
