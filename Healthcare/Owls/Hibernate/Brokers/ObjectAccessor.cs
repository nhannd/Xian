#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Helper class returned by <see cref="ObjectAccessor.GetPropertyAccessor"/>.
	/// </summary>
	abstract class PropertyAccessor
	{
		/// <summary>
		/// Gets the value of the property from the specified object instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public abstract object GetValue(object instance);
	}

	/// <summary>
	/// Helper class that provides efficient runtime binding to object property getters.
	/// </summary>
	/// <remarks>
	/// Internally, this class creates delegates that bind directly to the object property
	/// getter methods.  This avoids the overhead of using reflection repeatedly to
	/// read the same property on different object instances.
	/// </remarks>
	abstract class ObjectAccessor
	{
		/// <summary>
		/// Creates an instance of <see cref="ObjectAccessor"/> for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ObjectAccessor CreateObjectAccessor(Type type)
		{
			var accessorType = typeof(ObjectAccessor<>).MakeGenericType(new[] { type });
			return (ObjectAccessor)Activator.CreateInstance(accessorType);
		}

		/// <summary>
		/// Gets a property accessor for the public property with the specified name.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public abstract PropertyAccessor GetPropertyAccessor(string property);
	}

	/// <summary>
	/// Generic extension of <see cref="ObjectAccessor{T}"/>.  Do not use this class - use <see cref="ObjectAccessor.CreateObjectAccessor"/>
	/// factory method instead.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class ObjectAccessor<T> : ObjectAccessor
	{
		/// <summary>
		/// Generic implementation of <see cref="PropertyAccessor"/>.
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		class PropertyAccessor<TProperty> : PropertyAccessor
		{
			private delegate TProperty PropertyGetter(T instance);

			private readonly PropertyGetter _getter;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="property"></param>
			public PropertyAccessor(PropertyInfo property)
			{
				// create a delegate bound to the getter method of this property
				_getter = (PropertyGetter)Delegate.CreateDelegate(typeof(PropertyGetter), null, property.GetGetMethod());
			}

			/// <summary>
			/// Gets the value of the property from the specified object instance.
			/// </summary>
			/// <param name="instance"></param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				return _getter((T)instance);
			}
		}

		/// <summary>
		/// Gets a property accessor for the public property with the specified name.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public override PropertyAccessor GetPropertyAccessor(string property)
		{
			var prop = typeof(T).GetProperty(property);
			var accessorType = typeof(PropertyAccessor<>).MakeGenericType(new[] { typeof(T), prop.PropertyType });
			return (PropertyAccessor)Activator.CreateInstance(accessorType, prop);
		}
	}
}
