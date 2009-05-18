using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Common;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Caching;
using System.Reflection;

namespace ClearCanvas.Enterprise.Common
{
    public class AuthenticationClient : IAuthenticationService
    {
        public AuthenticationClient()
        {
        }


        #region IAuthenticationService Members

        public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
            return Invoke<InitiateSessionRequest, InitiateSessionResponse>(
                "InitiateSession", request, null);
        }

        public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
            return Invoke<ValidateSessionRequest, ValidateSessionResponse>(
                "ValidateSession", request, string.Concat(request.UserName, ":", request.SessionToken.Id));
        }

        public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
        {
            return Invoke<TerminateSessionRequest, TerminateSessionResponse>(
                "TerminateSession", request, null);
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            return Invoke<ChangePasswordRequest, ChangePasswordResponse>(
                "ChangePassword", request, null);
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            return Invoke<GetAuthorizationsRequest, GetAuthorizationsResponse>(
                "GetAuthorizations", request, string.Concat(request.UserName, ":", request.SessionToken.Id));
        }

        #endregion

        private TResponse Invoke<TRequest, TResponse>(string operation, TRequest request, string cacheKey)
            where TResponse: DataContractBase
            where TRequest : DataContractBase
        {
            string cacheID = typeof(IAuthenticationService).Name;

            TResponse response = null;
            if (cacheKey != null)
            {
                // check cache, and return if available
                response = CheckCache(operation, cacheKey) as TResponse;
                if (response != null)
                    return response;
            }

            // not available, so invoke service
            ResponseCachingDirective directive = null;
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    MethodInfo method = typeof(IAuthenticationService).GetMethod(operation);
                    if (service is IContextChannel)
                    {
                        using (OperationContextScope scope = new OperationContextScope(service as IContextChannel))
                        {
                            // invoke the operation
                            response = (TResponse)method.Invoke(service, new object[] { request });

                            // read caching directive from headers
                            int h = OperationContext.Current.IncomingMessageHeaders.FindHeader(
                                        ResponseCachingDirective.HeaderName,
                                        ResponseCachingDirective.HeaderNamespace);
                            if(h > -1)
                            {
                               directive = OperationContext.Current.IncomingMessageHeaders
                                    .GetHeader<ResponseCachingDirective>(h);
                            }
                        }
                    }
                    else
                    {
                        // invoke the operation
                        response = (TResponse)method.Invoke(service, new object[] { request });
                    }
                });

            // cache the response if directed
            if (cacheKey != null && directive != null && directive.EnableCaching)
            {
                CacheResponse(operation, cacheKey, response, directive.TimeToLive);
            }

            return response;
        }

        private object CheckCache(string operation, string cacheKey)
        {
            string cacheID = typeof(IAuthenticationService).Name;
            ICacheClient cacheClient = Cache.CreateClient(
                new CacheClientCreationArgs(cacheID, operation));

            return cacheClient.Get(cacheKey);
        }

        private void CacheResponse(string operation, string cacheKey, object response, TimeSpan ttl)
        {
            string cacheID = typeof(IAuthenticationService).Name;
            ICacheClient cacheClient = Cache.CreateClient(
                    new CacheClientCreationArgs(cacheID, operation, ttl, false));
            cacheClient.Put(cacheKey, response);
        }
    }
}
