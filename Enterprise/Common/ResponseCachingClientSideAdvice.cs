#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Caching;

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
			// Attempts to read the cache directive header from the message, returning null if the header doesn't exist.
			var h = OperationContext.Current.IncomingMessageHeaders.FindHeader(
						ResponseCachingDirective.HeaderName,
						ResponseCachingDirective.HeaderNamespace);
			if (h > -1)
			{
				return OperationContext.Current.IncomingMessageHeaders
					 .GetHeader<ResponseCachingDirective>(h);
			}

			return null;
		}
	}
}
