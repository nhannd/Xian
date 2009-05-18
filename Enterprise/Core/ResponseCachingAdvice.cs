using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Enterprise.Common;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core
{
    public class ResponseCachingAdvice : ServiceOperationAdvice, IInterceptor
    {
        public ResponseCachingAdvice()
        {

        }

        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {

            try
            {
                object retval = invocation.Proceed(args);

                // if the invocation succceeded, process the caching directive
                ProcessCacheDirective(invocation.InvocationTarget, invocation.MethodInvocationTarget, args);

                return retval;
            }
            finally
            {
            }
        }

        private static void ProcessCacheDirective(object service, MethodInfo operation, object[] args)
        {
            ResponseCachingDirective directive = GetCachingDirective(service, operation, args);
            if (directive == null)
                return;

            if (directive.CacheSite == ResponseCachingSite.Server)
                throw new NotSupportedException("Server-side caching not yet supported.");  // TODO implement this in future

            // if cache site is client, and we have an op context (eg running as a WCF service),
            // add caching directive to WCF message headers so that we send it to the client
            if (directive.CacheSite == ResponseCachingSite.Client && OperationContext.Current != null)
            {
                MessageHeader header = MessageHeader.CreateHeader(
                    ResponseCachingDirective.HeaderName,
                    ResponseCachingDirective.HeaderNamespace,
                    directive);
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
            }
        }

        private static ResponseCachingDirective GetCachingDirective(object service, MethodInfo operation, object[] args)
        {
             // determine if the response is cacheable, and if so, obtain caching directive
            ResponseCachingAttribute attr = AttributeUtils.GetAttribute<ResponseCachingAttribute>(operation, false);
            if (attr != null)
            {
                Type serviceClass = operation.DeclaringType;
                MethodInfo method = serviceClass.GetMethod(attr.DirectiveMethod,
                    BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static);
                return (ResponseCachingDirective)method.Invoke(service, args);
            }
            return null;
       }

        #endregion
    }
}
