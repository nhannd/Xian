using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Data
{
    public class ReadContextAdvice : PersistenceContextAdvice
    {
        internal ReadContextAdvice()
        {
        }

        public override object Invoke(IMethodInvocation invocation)
        {
            try
            {
                ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
                using (new PersistenceScope(PersistenceContextType.Read, a.PersistenceScopeOption))
                {
                    return invocation.Proceed();
                }
            }
            finally
            {
            }
        }
    }
}
