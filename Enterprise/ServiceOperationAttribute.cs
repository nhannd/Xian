using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ServiceOperationAttribute : Attribute
    {
        private PersistenceScopeOption _scopeOption;
        private bool _auditable;
        
        public ServiceOperationAttribute()
        {
            // operation is auditable by default
            _auditable = true;

            // a persistence context is required, by default
            _scopeOption = PersistenceScopeOption.Required;
        }

        public bool Auditable
        {
            get { return _auditable; }
            set { _auditable = value; }
        }

        public PersistenceScopeOption PersistenceScopeOption
        {
            get { return _scopeOption; }
            set { _scopeOption = value; }
        }
   }
}
