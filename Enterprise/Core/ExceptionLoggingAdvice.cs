#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;

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
