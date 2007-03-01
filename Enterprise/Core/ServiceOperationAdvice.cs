using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ClearCanvas.Enterprise.Core
{
    public abstract class ServiceOperationAdvice
    {
        internal ServiceOperationAdvice()
        {
        }

        protected ServiceOperationAttribute GetServiceOperationAttribute(MethodInfo method)
        {
            object[] attributes = method.GetCustomAttributes(typeof(ServiceOperationAttribute), true);
            Debug.Assert(attributes.Length == 1);
            return (ServiceOperationAttribute)attributes[0];
        }
    }
}
