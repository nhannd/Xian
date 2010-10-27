#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
