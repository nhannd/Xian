using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    public class ServiceMountException : Exception
    {
        public ServiceMountException(string message, Exception inner)
            :base(message, inner)
        {
        }

        public ServiceMountException(string message)
            :this(message, null)
        {
        }
    }
}
