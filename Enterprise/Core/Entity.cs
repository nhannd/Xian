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

		/// <summary>
		/// Gets the OID of this entity instance.
		/// </summary>
		/// <remarks>
		/// OID is short for object identifier, and is used to store the surrogate key that uniquely identifies the 
		/// object in the database.  This property is public for compatibility with NHibernate proxies.  It should
		/// not be used by application code.
		/// </remarks>
		public virtual object OID
		{
			get { return _oid; }
			// private setter for NH compatability
			private set { _oid = value; }
		}

		/// <summary>
		/// Gets the version of this entity instance.
		/// </summary>
		/// <remarks>
		/// Keeps track of the object version for optimistic concurrency.  This property is public for compatibility
		/// with NHibernate proxies.  It should not be used by application code.
		/// </remarks>
		public virtual int Version
		{
			get { return _version; }
			protected set { _version = value; }
		}

		/// <summary>
		/// Gets a <see cref="EntityRef"/> that refers to this entity.
		/// </summary>
		/// <returns></returns>
		public virtual EntityRef GetRef()
		{
			if (_oid == null)
				throw new InvalidOperationException("Cannot generate entity ref on transient entity");

			return new EntityRef(GetClass(), _oid, _version);
		}


		/// <summary>
		/// Performs a downcast on this object to the specified subclass type.
		/// </summary>
		/// <remarks>
		/// If this object is a proxy, a regular C# downcast operation will fail.
		/// Therefore, application code should always use this method to perform a safe downcast.
		/// </remarks>
		/// <typeparam name="TSubclass"></typeparam>
		/// <returns></returns>
		// note: this method must not be made virtual or Castle.DynamicProxy2 will try to proxy it
		public TSubclass Downcast<TSubclass>()
			where TSubclass : Entity
		{
			return (TSubclass)GetRawInstance();
		}

		/// <summary>
		/// Checks if this object is an instance of the specified subclass.
		/// </summary>
		/// <remarks>
		/// Subsitute for the C# 'is' operator.  If this object is a proxy, the C# 'is' operator will never
		/// return true.  Therefore, application code must use this method instead.
		/// </remarks>
		/// <typeparam name="TSubclass"></typeparam>
		/// <returns></returns>
		// note: this method must not be made virtual or Castle.DynamicProxy2 will try to proxy it
		public bool Is<TSubclass>()
			where TSubclass : Entity
		{
			return GetRawInstance() is TSubclass;
		}

		/// <summary>
		/// Performs a conditional cast of this instance to the specified subclass.
		/// </summary>
		/// <remarks>
		/// Subsitute for the C# 'as' operator.  If this object is a proxy, the C# 'as' operator will fail.
		/// Therefore, application code must use this method instead.
		/// </remarks>
		/// <typeparam name="TSubclass"></typeparam>
		/// <returns></returns>
		// note: this method must not be made virtual or Castle.DynamicProxy2 will try to proxy it
		public TSubclass As<TSubclass>()
			where TSubclass : Entity
		{
			return GetRawInstance() as TSubclass;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <remarks>
		/// Overridden to handle equality between proxy and raw instance correctly.
		/// </remarks>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
		///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
		///                 </exception><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			// handle the easy case right off
			if (ReferenceEquals(this, obj))
				return true;


			// false if obj is null, or not castable to Entity
			var that = obj as Entity;
			if (ReferenceEquals(that, null))
				return false;

			// handle the case where 'that' is a proxy, and 'this' is not
			return ReferenceEquals(this, that.GetRawInstance());
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <remarks>
		/// Overridden to handle equality between proxy and raw instance correctly.
		/// </remarks>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			// Although this override seems redundant, it is crucial to correct proxy handling.
			// Because GetHashCode is a virtual method, we know that it is proxied.
			// If we do not override it, the hashcode returned will be that of the proxy.
			// By overriding it and calling base, we ensure that the hashcode of the raw
			// instance is returned.  This this enforces that an entity and its proxy return
			// the same hash code.
			return base.GetHashCode();
		}
	}
}
