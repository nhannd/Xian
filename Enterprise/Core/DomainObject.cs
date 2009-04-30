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
        /// In the case where this object is a proxy, returns the raw instance underlying the proxy.  This
        /// method must be virtual for correct behaviour, however, it is not intended to be overridden by
        /// subclasses and is not intended for use by application code.
        /// </summary>
        /// <returns></returns>
        protected virtual DomainObject GetRawInstance()
        {
            return this;
        }

        /// <summary>
        /// Gets the domain class of this object.  Note that the domain class is not necessarily the same as the
        /// type of this object, because this object may be a proxy.  Therefore, use this method rather
        /// than <see cref="object.GetType"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetClass()
        {
            return GetRawInstance().GetType();
        }
    }
}
