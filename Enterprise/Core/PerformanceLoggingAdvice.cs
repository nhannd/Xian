#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
    public class PerformanceLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            DateTime startTime = DateTime.Now;
            invocation.Proceed();
            DateTime endTime = DateTime.Now;


            string operationName = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

            Platform.Log(LogLevel.Info, "{0} {1} - {2} (Duration = {3}",
                operationName,
                startTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                endTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                (endTime - startTime).ToString());
        }

        #endregion
    }
}
