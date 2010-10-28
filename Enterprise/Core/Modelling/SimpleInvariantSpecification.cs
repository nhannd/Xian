#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Base class for rules that represent simple invariant constraints on a property of an object.
	/// </summary>
	internal abstract class SimpleInvariantSpecification : ISpecification, IPropertyBoundRule
	{
		private readonly PropertyInfo[] _properties;

		protected SimpleInvariantSpecification(PropertyInfo[] properties)
		{
			_properties = properties;
		}

		protected SimpleInvariantSpecification(PropertyInfo property)
		{
			_properties = new [] { property };
		}


		#region ISpecification Members

		public abstract TestResult Test(object obj);

		#endregion


		public PropertyInfo[] Properties
		{
			get { return _properties; }
		}

		public PropertyInfo Property
		{
			get { return _properties[0]; }
		}

		protected object GetPropertyValue(object obj)
		{

			return GetPropertyValue(obj, _properties[0]);
		}

		protected object[] GetPropertyValues(object obj)
		{
			return CollectionUtils.Map(_properties, (PropertyInfo property) => GetPropertyValue(obj, property)).ToArray();
		}

		private static object GetPropertyValue(object obj, PropertyInfo property)
		{
			return property.GetGetMethod().Invoke(obj, null);
		}
	}
}
