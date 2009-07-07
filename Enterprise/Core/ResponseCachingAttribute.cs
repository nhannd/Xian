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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Applied to a service operation to indicate that the response is cacheable.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ResponseCachingAttribute : Attribute
    {
        private readonly string _directiveMethod;

		/// <summary>
		/// Constructor which accepts the name of a method on the service that supplies the <see cref="ResponseCachingDirective"/>
		/// for this operation.
		/// </summary>
		/// <param name="directiveMethod">See <cref="DirectiveMethod"/>.</param>
        public ResponseCachingAttribute(string directiveMethod)
        {
            _directiveMethod = directiveMethod;
        }

		/// <summary>
		/// Gets the name of a method on the service class that returns an instance of <see cref="ResponseCachingDirective"/>.
		/// </summary>
		/// <remarks>
		/// The named method must accept the same parameters as the service operation to which it is applied
		/// (or it may accept more general Types of those parameters e.g. objects), and must return
		/// an instance of <see cref="ResponseCachingDirective"/>.  For example, a typical signature
		/// would be this: 
		/// <code>
		///		ResponseCachingDirective MyMethod(object request)
		/// </code>
		/// When the service operation is invoked, this method will be invoked with the request object that was passed
		/// to the service.  Hence the caching directive may vary as a function of the request.
		/// </remarks>
        public string DirectiveMethod
        {
            get { return _directiveMethod; }
        }
    }
}
