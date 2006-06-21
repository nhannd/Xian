using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    public class ActionBuilderException : Exception
    {
        internal ActionBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
