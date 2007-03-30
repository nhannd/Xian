using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceAuthenticationAttribute : Attribute
    {
        private bool _requireAuthentication;

        public ServiceAuthenticationAttribute(bool requireAuthentication)
        {
            _requireAuthentication = requireAuthentication;
        }

        public bool RequireAuthentication
        {
            get { return _requireAuthentication; }
        }
    }
}
