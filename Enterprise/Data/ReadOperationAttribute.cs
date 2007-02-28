using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ReadOperationAttribute : ServiceOperationAttribute
    {
        public ReadOperationAttribute()
        {
        }
    }
}
