using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    public class ActionBuilderException : Exception
    {
        internal ActionBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
