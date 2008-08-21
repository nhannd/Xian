using System;
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsbile for logging unhandled exceptions thrown from service operation methods.
    /// </summary>
    public class ExceptionLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            try
            {
                return invocation.Proceed(args);
            }
            catch (Exception e)
            {
                ExceptionLogger.Log(
                    string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name), 
                    e);

                // rethrow the exception
                throw;
            }
        }

        #endregion
    }
}
