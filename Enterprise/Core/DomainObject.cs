#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

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
		/// Get the unproxied object reference.
		/// </summary>
		/// <remarks>
		/// In the case where this object is a proxy, returns the raw instance underlying the proxy.  This
		/// method must be virtual for correct behaviour, however, it is not intended to be overridden by
		/// subclasses and is not intended for use by application code.
		/// </remarks>
		/// <returns></returns>
		protected virtual DomainObject GetRawInstance()
		{
			// because GetRawInstance is virtual, 'this' refers to the raw instance
			return this;
		}

		/// <summary>
		/// Gets the domain class of this object.
		/// </summary>
		/// <remarks>
		/// The domain class is not necessarily the same as the type of this object, because this object may be a proxy.
		/// Therefore, use this method rather than <see cref="object.GetType"/>.
		/// </remarks>
		/// <returns></returns>
		public virtual Type GetClass()
		{
			// because GetClass is virtual, 'this' refers to the raw instance
			return this.GetType();
		}
	}
}
