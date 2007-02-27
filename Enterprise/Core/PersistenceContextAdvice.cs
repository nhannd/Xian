using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Core
{
    public abstract class PersistenceContextAdvice : ServiceOperationAdvice, IMethodInterceptor
    {
        internal PersistenceContextAdvice()
        {
        }

        public abstract object Invoke(IMethodInvocation invocation);
    }
}
