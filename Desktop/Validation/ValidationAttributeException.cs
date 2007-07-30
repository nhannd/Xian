using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidationAttributeException : Exception
    {
        public ValidationAttributeException(string message)
            :base(message)
        {
        }
    }
}
