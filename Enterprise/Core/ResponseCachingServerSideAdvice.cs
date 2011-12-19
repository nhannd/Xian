#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using System.Reflection;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice to implement transparent caching on the callee side, 
	/// or create an appropriate <see cref="ResponseCachingDirective"/> to send back to the callee.
	/// </summary>
	public class ResponseCachingServerSideAdvice : ResponseCachingAdviceBase, IInterceptor
	{
		#region IInterceptor Members

		void IInterceptor.Intercept(IInvocation invocation)
		{
			ProcessInvocation(invocation, "ServerSideResponseCache");
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Gets the caching directive.
		/// </summary>
		protected override ResponseCachingDirective GetCachingDirective(IInvocation invocation)
		{
			// Dynamically invokes the specified method to obtain the caching directive.
			var service = invocation.InvocationTarget;
			var operation = invocation.MethodInvocationTarget;

			// determine if the response is cacheable, and if so, obtain caching directive
			var attr = AttributeUtils.GetAttribute<ResponseCachingAttribute>(operation, false);
			if (attr != null)
			{
				var serviceClass = operation.DeclaringType;
				var method = serviceClass.GetMethod(attr.DirectiveMethod,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

				return (ResponseCachingDirective)method.Invoke(service, GetDirectiveMethodArgs(invocation, method));
			}
			return null;
		}

		/// <summary>
		/// Implemented by the subclass to cache the response, based on the specified caching directive.
		/// </summary>
		protected override void CacheResponse(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region, ResponseCachingDirective directive)
		{
			// if site is server (e.g. callee), put it in our cache
			if (directive.CacheSite == ResponseCachingSite.Server)
			{
				PutResponseInCache(invocation, cacheClient, cacheKey, region, directive);
				return;
			}

			// if site is client (e.g. caller), send directive to client
			if (directive.CacheSite == ResponseCachingSite.Client)
			{
				// check if we have an op context (eg we are running as a WCF service)
				// if not, then this is not applicable
				if (OperationContext.Current == null)
					return;

				// send cache directive to client via headers
				WriteCachingDirectiveHeaders(directive, OperationContext.Current);
				return;
			}
		}

		#endregion

		private static object[] GetDirectiveMethodArgs(IInvocation invocation, MethodInfo method)
		{
			// method is expected have one of the following signatures:
			// ResponseCachingDirective A()
			// ResponseCachingDirective B(object request)
			// ResponseCachingDirective C(object request, object response)
			
			// the args list is prepared to match the signature
			switch (method.GetParameters().Length)
			{
				case 0:
					return null;
				case 1:
					return new[] { invocation.Arguments[0] };
				case 2:
					return new[] { invocation.Arguments[0], invocation.ReturnValue };
				default:
					throw new InvalidOperationException(string.Format("Method {0} does have a signature that is supported.", method.Name));
			}
		}
	}
}
