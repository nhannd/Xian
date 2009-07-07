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
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Enterprise.Common;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for honouring the <see cref="ResponseCachingAttribute"/> that may
	/// be applied to service operations.
	/// </summary>
    public class ResponseCachingAdvice : ServiceOperationAdvice, IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            // if the invocation succceeded, process the caching directive
            ProcessCacheDirective(invocation.InvocationTarget, invocation.MethodInvocationTarget, invocation.Arguments);
        }

		/// <summary>
		/// Processes any caching directive specified by the attribute.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="operation"></param>
		/// <param name="args"></param>
        private static void ProcessCacheDirective(object service, MethodInfo operation, object[] args)
        {
            ResponseCachingDirective directive = GetCachingDirective(service, operation, args);
            if (directive == null || Equals(directive, ResponseCachingDirective.DoNotCacheDirective))
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

		/// <summary>
		/// Dynamically invokes the specified method to obtain the caching directive.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="operation"></param>
		/// <param name="args"></param>
		/// <returns></returns>
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
