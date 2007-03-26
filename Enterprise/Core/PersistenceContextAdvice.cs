using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Core
{
    public class PersistenceContextAdvice : ServiceOperationAdvice, IMethodInterceptor
    {
        internal PersistenceContextAdvice()
        {
        }

        public object Invoke(IMethodInvocation invocation)
        {
            object retval = null;

            try
            {
                ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
                using (PersistenceScope scope = a.CreatePersistenceScope())
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
