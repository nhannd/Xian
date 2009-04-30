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
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base class for all entities in the domain model.
    /// </summary>
    /// 
    // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    // All parent classes must be serializable
    [Serializable] 
    public abstract class Entity : DomainObject
    {
        private object _oid;
        private int _version;

        public Entity()
        {
        }

        /// <summary>
        /// OID is short for object identifier, and is used to store the surrogate key that uniquely identifies the 
        /// object in the database.  This property is public for compatibility with NHibernate proxies.  It should
        /// not be used by application code.
        /// </summary>
        public virtual object OID
        {
            get { return _oid; }
            private set { _oid = value; }
        }

        /// <summary>
        /// Keeps track of the object version for optimistic concurrency.  This property is public for compatibility
        /// with NHibernate proxies.  It should not be used by application code.
        /// </summary>
        public virtual int Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        /// <summary>
        /// Gets a <see cref="EntityRef"/> that represents this entity.
        /// </summary>
        /// <returns></returns>
        public virtual EntityRef GetRef()
        {
            if (_oid == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return new EntityRef(GetClass(), _oid, _version);
        }


        /// <summary>
        /// Performs a downcast on this object to the specified subclass type.  If this object is a proxy,
        /// a regular C# downcast operation will fail.  Therefore, application code should always use this method
        /// to perform a safe downcast.
        /// </summary>
        /// <typeparam name="TSubclass"></typeparam>
        /// <returns></returns>
        // Note this method must not be made virtual or Castle.DynamicProxy will try to proxy it
        public TSubclass Downcast<TSubclass>()
            where TSubclass : Entity
        {
            return (TSubclass)GetRawInstance();
        }

        /// <summary>
        /// Subsitute for the C# 'is' operator.  If this object is a proxy, the C# 'is' operator will never
        /// return true.  Therefore, application code must use this method instead.
        /// </summary>
        /// <typeparam name="TSubclass"></typeparam>
        /// <returns></returns>
        // Note this method must not be made virtual or Castle.DynamicProxy will try to proxy it
        public bool Is<TSubclass>()
            where TSubclass : Entity
        {
            return GetRawInstance() is TSubclass;
        }

        /// <summary>
        /// Subsitute for the C# 'as' operator.  If this object is a proxy, the C# 'as' operator will fail.
        /// Therefore, application code must use this method instead.
        /// </summary>
        /// <typeparam name="TSubclass"></typeparam>
        /// <returns></returns>
        // Note this method must not be made virtual or Castle.DynamicProxy will try to proxy it
        public TSubclass As<TSubclass>()
            where TSubclass : Entity
        {
            return GetRawInstance() as TSubclass;
        }
    }
}
