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
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Base class for <see cref="Entity"/>, <see cref="ValueObject"/> and <see cref="EnumValue"/>.
    /// </summary>
    /// 
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public abstract class DomainObject
    {
        /// <summary>
        /// In the case where the called instance is a proxy, returns the proxy target object.
		/// If the called instance is not a proxy, returns this instance.
        /// </summary>
		/// <remarks>
		/// This method must be virtual for correct behaviour, however, it is not intended to be overridden by
		/// subclasses and is not intended for use by application code.
		/// </remarks>
        /// <returns></returns>
        protected virtual DomainObject GetRawInstance()
        {
			// when called through a class proxy, the proxy method will call through to this method
			// which ensures that "this" refers to the proxy target object, and not the proxy itself
            return this;
        }

        /// <summary>
        /// Gets the domain class of this object.
        /// </summary>
		/// <remarks>
		/// Note that the domain class is not necessarily the same as the type of this object,
		/// because this object may be a proxy.  Therefore, use this method rather than
		/// <see cref="object.GetType"/>.
		/// </remarks>
        /// <returns></returns>
        public virtual Type GetClass()
        {
            return GetRawInstance().GetType();
        }

		/// <summary>
		/// Overridden to provide reference equality even when one of the instances is a proxy.
		/// </summary>
		/// <remarks>
		/// Given an object X and a proxy X' that targets X, the comparison X == X' will return false.
		/// Object.Equals(X, X') would also return false, because the default implementation of 
		/// <see cref="Object.Equals"/> does a reference comparison.  In order to overcome this,
		/// DomainObject overrides Equals to ensure that Object.Equals(X, X') returns true.
		/// </remarks>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var that = obj as DomainObject;
			if (that == null)
				return false;

			// compare raw instances, just in case "that" is a proxy
			return ReferenceEquals(GetRawInstance(), that.GetRawInstance());
		}

		/// <summary>
		/// Overridden to ensure that a proxy and its target return the same hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return GetRawInstance().GetHashCode();
		}
    }
}
