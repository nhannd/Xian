using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UpdateOperationAttribute : ServiceOperationAttribute
    {
        public UpdateOperationAttribute()
        {
            // update operations are auditable by default
            this.Auditable = true;
        }
    }
}
