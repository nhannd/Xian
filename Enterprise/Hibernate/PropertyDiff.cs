#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using ClearCanvas.Enterprise.Common;
using NHibernate.Type;
using ClearCanvas.Common.Utilities;
using NHibernate;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Used by <see cref="ChangeRecord"/> to record changes to individual properties.
	/// </summary>
	class PropertyDiff
	{
		private readonly string _propertyName;
		private readonly object _oldValue;
		private readonly object _newValue;
		private readonly IType _hibernateType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="hibernateType"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		internal PropertyDiff(string propertyName, IType hibernateType, object oldValue, object newValue)
		{
			_propertyName = propertyName;
			_hibernateType = hibernateType;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public string PropertyName
		{
			get { return _propertyName; }
		}

		public bool IsCollectionProperty
		{
			get { return _hibernateType.IsCollectionType; }
		}

		public bool IsChanged
		{
			get
			{
				// if we're dealing with a collection property
				if (IsCollectionProperty)
				{
					// two collections are effectively equal if their contents are equal (we do not even care if they are the same instance)
					// however, we also want to avoid lazy loading an uninitialized collection if possible.
					// if both values refer to the same instance, and this instance is uninitialized, we know the contents
					// have not changed, and a content-comparison is not needed
					if (ReferenceEquals(_oldValue, _newValue) && !NHibernateUtil.IsInitialized(_newValue))
						return false;

					// otherwise, we need to compare collection contents (even if it means causing a lazy-load to occur)
					//TODO: collections with list semantics should use order-sensitive comparisons, but how do we know??
					//(e.g how do we differentiate a "bag" from a "list"?)
					return !CollectionUtils.Equal((ICollection)_oldValue, (ICollection)_newValue, false);
				}

				// if we're dealing with an entity-ref property
				if (_hibernateType.IsEntityType)
				{
					if (_oldValue is Entity || _newValue is Entity)
					{
						// ensure we do an efficient comparison that does not cause unnecessary proxy initialization
						return !EqualityUtils<Entity>.AreEqual((Entity)_oldValue, (Entity)_newValue);
					}
					if (_oldValue is EnumValue || _newValue is EnumValue)
					{
						// ensure we do an efficient comparison that does not cause unnecessary proxy initialization
						return !EqualityUtils<EnumValue>.AreEqual((EnumValue)_oldValue, (EnumValue)_newValue);
					}
				}

				// use standard equality check
				return !Equals(_oldValue, _newValue);
			}
		}

		public PropertyChange AsPropertyChange()
		{
			return new PropertyChange(_propertyName, IsCollectionProperty, _oldValue, _newValue);
		}

		/// <summary>
		/// Returns a new <see cref="PropertyDiff"/> that is the result of adding this change
		/// to <paramref name="previousChange"/>.
		/// </summary>
		/// <param name="previousChange"></param>
		/// <returns></returns>
		/// <remarks>
		/// This operation is not commutative.
		/// </remarks>
		public PropertyDiff Compound(PropertyDiff previousChange)
		{
			return new PropertyDiff(_propertyName, _hibernateType, previousChange._oldValue, _newValue);
		}
	}
}
