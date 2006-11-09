using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using Spring.Aop.Advice;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Advises the session's transaction monitor to post the pending entity change notifications
    /// </summary>
    public class TransactionMonitorAdvice : ServiceOperationAdvice, IAfterReturningAdvice
    {
        internal TransactionMonitorAdvice(Session session)
            : base(session)
        {
        }

        #region IAfterReturningAdvice Members

        public void AfterReturning(object returnValue, System.Reflection.MethodInfo method, object[] args, object target)
        {
            //this.Session.TransactionNotifier.PostPending();
        }

        #endregion
    }
}
