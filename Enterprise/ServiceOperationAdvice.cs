using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ClearCanvas.Enterprise
{
    public abstract class ServiceOperationAdvice
    {
        private Session _session;

        internal ServiceOperationAdvice(Session session)
        {
            _session = session;
        }

        internal Session Session
        {
            get { return _session; }
        }

        protected ServiceOperationAttribute GetServiceOperationAttribute(MethodInfo method)
        {
            object[] attributes = method.GetCustomAttributes(typeof(ServiceOperationAttribute), true);
            Debug.Assert(attributes.Length == 1);
            return (ServiceOperationAttribute)attributes[0];
        }
    }
}
