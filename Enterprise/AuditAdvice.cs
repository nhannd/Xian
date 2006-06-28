using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;


namespace ClearCanvas.Enterprise
{
    public class AuditAdvice : ServiceOperationAdvice, IMethodInterceptor
    {
        public AuditAdvice(Session session)
            : base(session)
        {
        }

        #region IMethodInterceptor Members

        public object Invoke(IMethodInvocation invocation)
        {
            Exception error = null;
            object retval = null;
            try
            {
                retval = invocation.Proceed();
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
                if (a.Auditable)
                {
                    Console.WriteLine(string.Format("audit:  call to {0} {1}", invocation.Method.Name, error == null ? "Completed" : "Failed"));
                }

                if (error != null)
                    throw error;
            }
            return retval;
        }

        #endregion
    }
}
