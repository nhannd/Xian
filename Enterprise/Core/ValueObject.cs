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
	/// Base class for domain objects that behave as value types (NHiberate: components, or collection of values).
	/// </summary>
	public abstract class ValueObject : DomainObject, ICloneable
	{
		#region ICloneable Members

		public abstract object Clone();

		#endregion
	}
}
