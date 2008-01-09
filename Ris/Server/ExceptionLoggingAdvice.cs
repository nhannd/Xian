using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Server
{
    class ExceptionLoggingAdvice : IInterceptor
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
                        DefaultExceptionLogger logger = new DefaultExceptionLogger();
                        ExceptionLogEntry logEntry = logger.CreateExceptionLogEntry(e);

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
