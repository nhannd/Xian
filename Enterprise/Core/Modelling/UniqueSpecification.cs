#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies that a given property of an object is unique within the set of persistent instances of that object's class.
	/// </summary>
	/// <remarks>
	/// This class is similar to <see cref="UniqueKeySpecification"/>, except that the unique key is limited to a single
	/// primitive-valued property.  However, this limitation allows this class to implement <see cref="IPropertyBoundRule"/>
	/// since the key value is a function of a single property.
	/// </remarks>
	internal class UniqueSpecification : UniqueKeySpecification, IPropertyBoundRule
	{
		private readonly PropertyInfo _property;

		internal UniqueSpecification(PropertyInfo property)
			: base(property.DeclaringType, property.Name, new[] { property.Name })
		{
			_property = property;
		}

		#region IPropertyBoundRule Members

		public PropertyInfo[] Properties
		{
			get { return new[] { _property }; }
		}

		#endregion
	}
}
