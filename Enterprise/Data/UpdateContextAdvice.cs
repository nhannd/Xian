using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Data
{
    public class UpdateContextAdvice : PersistenceContextAdvice
    {
        internal UpdateContextAdvice()
        {
        }

        public override object Invoke(IMethodInvocation invocation)
        {
            object retval = null;

            try
            {
                ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, a.PersistenceScopeOption))
                {
                    retval = invocation.Proceed();
                    
                    // auto-commit transaction
                    scope.Complete();
                }

                return retval;
            }
            finally
            {
            }
        }
    }
}
