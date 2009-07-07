#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Client-side advice to implement transparent caching on the client side,
	/// where the server returns an appropriate <see cref="ResponseCachingDirective"/>.
	/// </summary>
    class ResponseCachingClientAdvice : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            object service = invocation.InvocationTarget;
            string region = string.Format("{0}.{1}",
                invocation.Method.DeclaringType.FullName, invocation.Method.Name);
            object request = invocation.Arguments[0];

			// check for a cached response
			// must do this even if this operation does not support caching, because have no way of knowing
			// whether it does or does not support caching
            string cacheKey = GetCacheKey(request);
			using (ICacheClient cacheClient = Cache.CreateClient("ResponseCache"))
			{
				if (cacheKey != null && cacheClient.RegionExists(region))
				{
					// check cache
				    object response = cacheClient.Get(cacheKey, new CacheGetOptions(region));
                    if(response != null)
                    {
                        invocation.ReturnValue = response;
                        return;
                    }
				}

				// no cached response is available, so invoke service operation
				using (new OperationContextScope(service as IContextChannel))
				{
					// invoke the operation
                    invocation.Proceed();

					// read caching directive from headers
					ResponseCachingDirective directive = ReadCachingDirectiveHeader();

					// cache the response if directed
					if (directive != null && directive.EnableCaching)
					{
						// if we didn't succeed in getting a cache key, this is an error
						if (cacheKey == null)
							throw new InvalidOperationException(
								string.Format("{0} is cacheable but the request class does not implement IDefinesCacheKey.", invocation.GetType().FullName));

						// cache the response for future use
						cacheClient.Put(cacheKey, invocation.ReturnValue, new CachePutOptions(region, directive.TimeToLive, false));
					}
				}
			}
        }

		/// <summary>
		/// Obtains the cache key for the specified request object.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
        private static string GetCacheKey(object request)
        {
            if (request is IDefinesCacheKey)
            {
                return (request as IDefinesCacheKey).GetCacheKey();
            }
            // for now, the request must implement an interface to get a cache key
            // in future, we could add some automatic serialization to turn it into a cache key
            return null;
        }

		/// <summary>
		/// Attempts to read the cache directive header from the message, returning null if the header doesn't exist.
		/// </summary>
		/// <returns></returns>
        private static ResponseCachingDirective ReadCachingDirectiveHeader()
        {
            int h = OperationContext.Current.IncomingMessageHeaders.FindHeader(
                        ResponseCachingDirective.HeaderName,
                        ResponseCachingDirective.HeaderNamespace);
            if (h > -1)
            {
                return OperationContext.Current.IncomingMessageHeaders
                     .GetHeader<ResponseCachingDirective>(h);
            }

            return null;
        }

        #endregion
    }
}
