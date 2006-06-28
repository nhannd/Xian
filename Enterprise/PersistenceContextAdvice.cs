using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise
{
    public abstract class PersistenceContextAdvice : ServiceOperationAdvice, IMethodInterceptor
    {
        public PersistenceContextAdvice(Session session)
            : base(session)
        {
        }

        public abstract object Invoke(IMethodInvocation invocation);
    }
}
