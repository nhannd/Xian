using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

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
                try
                {
                    // log the error to the database
                    using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
                    {
                        // disable change-set auditing for this context
                        ((IUpdateContext) PersistenceScope.Current).ChangeSetRecorder = null;

                        string operationName = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

                        DefaultExceptionRecorder recorder = new DefaultExceptionRecorder();
                        ExceptionLogEntry logEntry = recorder.CreateLogEntry(operationName, e);

                        PersistenceScope.Current.Lock(logEntry, DirtyState.New);

                        scope.Complete();
                    }
                }
                catch (Exception x)
                {
                    // if we fail to properly log the exception, there is nothing we can do about it
                    // just log a message to the log file
                    Platform.Log(LogLevel.Error, x);
                }

                // rethrow the exception
                throw;
            }
        }

        #endregion
    }
}
