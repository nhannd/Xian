using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Core
{
    public class UpdateContextAdvice : PersistenceContextAdvice
    {
        internal UpdateContextAdvice()
        {
        }

        public override object Invoke(IMethodInvocation invocation)
        {
            ServiceLayer serviceLayer = (ServiceLayer)invocation.This;
            object retval = null;

            try
            {
                ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, a.PersistenceScopeOption))
                {
                    // set the current context of the service layer
                    serviceLayer.CurrentContext = PersistenceScope.Current;
                    retval = invocation.Proceed();
                    
                    // auto-commit transaction
                    scope.Complete();
                }

                return retval;
            }
            finally
            {
                // be sure to remove the context from the service layer
                serviceLayer.CurrentContext = null;
            }
        }
    }
}
