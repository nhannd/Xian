using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ReadOperationAttribute : ServiceOperationAttribute
    {
        public ReadOperationAttribute()
        {
        }

        public override PersistenceScope CreatePersistenceScope()
        {
            return new PersistenceScope(PersistenceContextType.Read, this.PersistenceScopeOption);
        }
    }
}
