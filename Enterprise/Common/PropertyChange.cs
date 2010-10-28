#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Represents a change made to a property value of an entity.
	/// </summary>
	public class PropertyChange
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="isCollection"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		public PropertyChange(string propertyName, bool isCollection, object oldValue, object newValue)
		{
			PropertyName = propertyName;
			OldValue = oldValue;
			NewValue = newValue;
			IsCollection = isCollection;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public string PropertyName { get; private set; }

		/// <summary>
		/// Gets the old value of the property.
		/// </summary>
		public object OldValue { get; private set; }

		/// <summary>
		/// Gets the new value of the property.
		/// </summary>
		public object NewValue { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the property is a collection property.
		/// </summary>
		public bool IsCollection { get; private set; }

		/// <summary>
		/// Returns a new <see cref="PropertyChange"/> that is the result of adding this change
		/// to <paramref name="previousChange"/>.
		/// </summary>
		/// <param name="previousChange"></param>
		/// <returns></returns>
		/// <remarks>
		/// This operation is not commutative.
		/// </remarks>
		public PropertyChange AddTo(PropertyChange previousChange)
		{
			return new PropertyChange(PropertyName, IsCollection, previousChange.OldValue, NewValue);
		}
	}
}
