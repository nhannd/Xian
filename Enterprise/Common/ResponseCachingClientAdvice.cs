using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Common
{
    public class ResponseCachingClientAdvice : IInterceptor
    {
        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            object service = invocation.InvocationTarget;
            string region = string.Format("{0}.{1}",
                invocation.Method.DeclaringType.FullName, invocation.Method.Name);
            object request = args[0];
            object response = null;

            string cacheKey = GetCacheKey(request);
            ICacheClient cacheClient = Cache.CreateClient("ResponseCache");
            if (cacheKey != null && cacheClient.RegionExists(region))
            {
                // check cache, and return if available
                response = cacheClient.Get(cacheKey, new CacheGetOptions(region));
                if (response != null)
                    return response;
            }

            // not available, so invoke service
            using (OperationContextScope scope = new OperationContextScope(service as IContextChannel))
            {
                // invoke the operation
                response = invocation.Proceed(args);

                // read caching directive from headers
                ResponseCachingDirective directive = ReadCachingDirectiveHeader();

                // cache the response if directed
                if (directive != null && directive.EnableCaching)
                {
                    // if we didn't succeed in getting a cache key, this is an error
                    if (cacheKey == null)
                        throw new InvalidOperationException(
                            string.Format("{0} is cacheable but the request class does not implement ICacheKeyProvider.", response.GetType().FullName));

                    cacheClient.Put(cacheKey, response, new CachePutOptions(region, directive.TimeToLive, false));
                }
            }

            return response;

        }

        private string GetCacheKey(object request)
        {
            if (request is ICacheKeyProvider)
            {
                return (request as ICacheKeyProvider).GetCacheKey();
            }
            // for now, the request must implement an interface to get a cache key
            // in future, we could add some automatic serialization to turn it into a cache key
            return null;
        }

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
