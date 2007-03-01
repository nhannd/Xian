using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;


namespace ClearCanvas.Enterprise.Core
{
    public class AuditAdvice : ServiceOperationAdvice, IMethodInterceptor
    {
        internal AuditAdvice()
            : base()
        {
        }

        #region IMethodInterceptor Members

        public object Invoke(IMethodInvocation invocation)
        {
            ServiceOperationAttribute a = GetServiceOperationAttribute(invocation.Method);
            if (a.Auditable)
            {
                // only install a TransactionRecorder if the current context does not already have one
                if (PersistenceScope.Current != null && PersistenceScope.Current.TransactionRecorder == null)
                {
                    string transactionName = string.Format("{0}.{1}", invocation.This.GetType().FullName, invocation.Method.Name);
                    PersistenceScope.Current.TransactionRecorder = new DefaultTransactionRecorder(transactionName);
                }
            }
            return invocation.Proceed();
        }

        #endregion
    }
}
