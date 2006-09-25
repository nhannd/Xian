using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Scripting
{
    public class ActiveTemplateException : Exception
    {
        public ActiveTemplateException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
}
