#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using Castle.DynamicProxy;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsbile for logging unhandled exceptions thrown from service operation methods.
    /// </summary>
    public class ExceptionLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
			catch(FaultException e)
			{
				// if it was promoted to a fault, it isn't really an error as far as the server is concerned,
				// but more of an expected type of failure that is being communicated back to the client
				// Log at info level, and don't include stack trace since it isn't necessary
				var operation = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);
				var faultType = e.GetType().IsGenericType ? e.GetType().GetGenericArguments()[0] : null;
				Platform.Log(LogLevel.Info, "Fault ({0}): {1} ({2})", faultType != null ? faultType.Name : "unknown", e.Message, operation);

				// rethrow the exception so the fault gets to the client
				throw;
			}
            catch (Exception e)
            {
				Platform.Log(LogLevel.Error, e);

                // rethrow the exception so that the client gets an error
                throw;
            }
        }

        #endregion
    }
}
