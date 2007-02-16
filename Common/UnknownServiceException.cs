using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public class UnknownServiceException : Exception
    {
        public UnknownServiceException(string message)
            :base(message)
        {
        }
    }
}
