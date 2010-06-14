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

using System;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using System.Reflection;
using ClearCanvas.Enterprise.Common.Caching;

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
