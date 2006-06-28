using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ServiceOperationAttribute : Attribute
    {
        private bool _auditable;
        
        public ServiceOperationAttribute()
        {
            _auditable = true;  // operation is auditable by default
        }

        public bool Auditable
        {
            get { return _auditable; }
            set { _auditable = value; }
        }
    }
}
