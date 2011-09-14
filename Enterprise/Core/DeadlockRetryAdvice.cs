#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using Castle.Core.Interceptor;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice that will respond to a database deadlock by re-trying an operation several times. 
	/// </summary>
	public class DeadlockRetryAdvice : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			using (var retryManager = new DeadlockRetryManager())
			{
				retryManager.Execute(invocation.Proceed);
			}
		}
	}
}
