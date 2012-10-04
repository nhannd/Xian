#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Encapsulates a mapping between a given class of view item and the database table that it maps to.
	/// </summary>
	/// <remarks>
	/// This is a helper class that assists in efficiently populating views using the SQL bulk import functionality.
	/// </remarks>
	internal class ViewItemTableMapping
	{
		private readonly PersistentClass _classMapping;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="entityClass"></param>
		internal ViewItemTableMapping(Configuration config, Type entityClass)
		{
			_classMapping = config.GetClassMapping(entityClass);

			this.TableName = _classMapping.Table.Name;
			this.ColumnNames = ListColumns();
			this.ColumnValueProviders = CreatePropertyGetters();
		}

		#region Public API

		/// <summary>
		/// Gets the name of the database table that the specified view item class is mapped to.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		internal static string GetTableForClass(Type viewItemClass, Configuration configuration)
		{
			// we know that a leaf viewItem class maps to a single table
			return configuration.GetClassMapping(viewItemClass).Table.Name;
		}


		/// <summary>
		/// Gets the name of the table that this entity is mapped to.
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// Gets the ordered list of column names.
		/// </summary>
		public IList<string> ColumnNames { get; private set; }

		/// <summary>
		/// Gets the ordered list of column value provider functions.
		/// </summary>
		public IList<Converter<object, object>> ColumnValueProviders { get; private set; }

		#endregion

		#region Helpers

		/// <summary>
		/// Lists the columns in the table.
		/// </summary>
		/// <returns></returns>
		private IList<string> ListColumns()
		{
			var table = _classMapping.Table;
			return CollectionUtils.Map(table.ColumnIterator, (Column c) => c.Name);
		}

		/// <summary>
		/// Creates a list of functions that can be used to read property values from a view item instance.
		/// The list returned is aligned with that returned by <see cref="ListColumns"/>.
		/// </summary>
		/// <returns></returns>
		private IList<Converter<object, object>> CreatePropertyGetters()
		{
			var getters = CreatePropertyGetters(_classMapping.MappedClass, _classMapping.PropertyClosureIterator);

			var oidAccessor = ObjectAccessor.CreateObjectAccessor(_classMapping.MappedClass).GetPropertyAccessor("OID");
			getters.Insert(0, oidAccessor.GetValue);

			return getters;
		}

		/// <summary>
		/// Helper method to create property getter functions for all mapped properties of the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		private static IList<Converter<object, object>> CreatePropertyGetters(Type type, IEnumerable<Property> properties)
		{
			var propertyGetters = new List<Converter<object, object>>();

			var objAccessor = ObjectAccessor.CreateObjectAccessor(type);
			foreach (var prop in properties)
			{
				var propAccessor = objAccessor.GetPropertyAccessor(prop.Name);
				Converter<object, object> getter = propAccessor.GetValue;
				if (prop.Value is Component)
				{
					// recur on component properties
					var comp = prop.Value as Component;
					var subGetters = CreatePropertyGetters(comp.ComponentClass, comp.PropertyIterator);
					var dereferencedSubGetters = CollectionUtils.Map<Converter<object, object>, Converter<object, object>>(
						subGetters,
						g => delegate(object target)
						{
							var x = getter(target);
							return x == null ? null : g(x);
						});
					propertyGetters.AddRange(dereferencedSubGetters);
				}
				else
				{
					propertyGetters.Add(getter);
				}
			}
			return propertyGetters;
		}

		#endregion
	}
}
