using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsbile for logging unhandled exceptions thrown from service operation methods.
    /// </summary>
    class PerformanceLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            DateTime startTime = DateTime.Now;
            object result = invocation.Proceed(args);
            DateTime endTime = DateTime.Now;


            string operationName = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

            Platform.Log(LogLevel.Info, "{0} {1} - {2} (Duration = {3}",
                operationName,
                startTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                endTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                (endTime - startTime).ToString());

            return result;
        }

        #endregion
    }
}
