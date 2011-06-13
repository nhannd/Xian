#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Advice to implement transparent caching on the caller side,
	/// where the callee returns an appropriate <see cref="ResponseCachingDirective"/>.
	/// </summary>
	class ResponseCachingClientSideAdvice : ResponseCachingAdviceBase, IInterceptor
	{
		#region IInterceptor Members

		void IInterceptor.Intercept(IInvocation invocation)
		{
			// establish an operation context scope, in case we need to read message headers 
			var service = invocation.InvocationTarget;
			using (new OperationContextScope((IContextChannel)service))
			{
				ProcessInvocation(invocation, "ClientSideResponseCache");
			}
		}

		#endregion

		/// <summary>
		/// Implemented by the subclass to cache the response, based on the specified caching directive.
		/// </summary>
		protected override void CacheResponse(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region, ResponseCachingDirective directive)
		{
			// put the response in the local cache
			PutResponseInCache(invocation, cacheClient, cacheKey, region, directive);
		}

		/// <summary>
		/// Gets the caching directive.
		/// </summary>
		protected override ResponseCachingDirective GetCachingDirective(IInvocation invocation)
		{
			// read cache directive from server
			return ReadCachingDirectiveHeaders(OperationContext.Current);
		}
	}
}
