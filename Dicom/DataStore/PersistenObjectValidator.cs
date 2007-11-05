using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Type;

namespace ClearCanvas.Dicom.DataStore
{
	internal class PersistenObjectValidator
	{
		private readonly Configuration _configuration;
		private Dictionary<Type, Dictionary<string, PropertyInfo>> _persistentTypesPropertyInfo = null;

		public PersistenObjectValidator(Configuration configuration)
		{
			_configuration = configuration;
		}

		private IDictionary<string, PropertyInfo> GetPropertyInfo(Type type)
		{
			if (_persistentTypesPropertyInfo == null)
				_persistentTypesPropertyInfo = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

			if (_persistentTypesPropertyInfo.ContainsKey(type))
			{
				return _persistentTypesPropertyInfo[type];
			}
			else
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
				PropertyInfo[] propertyInfo = type.GetProperties(flags);
				Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
				foreach (PropertyInfo info in propertyInfo)
					properties[info.Name] = info;

				_persistentTypesPropertyInfo[type] = properties;
				return properties;
			}
		}

		private static void ValidateStringValue(Column column, object sourceObject, PropertyInfo info)
		{
			if (info.PropertyType == typeof(string))
			{
				string value = info.GetValue(sourceObject, null) as string;
				if (value == null && !column.IsNullable)
					throw new DataStoreException(String.Format(SR.FormatColumnIsNotNullable, column.Name, value));
				else if (String.IsNullOrEmpty(value) && column.IsUnique)
					throw new DataStoreException(String.Format(SR.FormatColumnMustBeUnique, column.Name, value));
				else if (value != null && value.Length > column.Length)
					throw new DataStoreException(String.Format(SR.FormatValueExceedsMaximumLength, value, column.Length, column.Name));
			}
		}
		public void ValidatePersistentObject(object obj)
		{
			PersistentClass pclass = _configuration.GetClassMapping(obj.GetType());
			IDictionary<string, PropertyInfo> pclassProperties = GetPropertyInfo(obj.GetType());

			foreach (Property property in pclass.PropertyClosureCollection)
			{
				ICollection columns = property.ColumnCollection;

				if (property.Type.IsComponentType)
				{
					IDictionary<string, PropertyInfo> properties = GetPropertyInfo(property.Type.ReturnedClass);
					object sourceObject = pclassProperties[property.Name].GetValue(obj, null);
					if (sourceObject == null)
						continue;

					int i = 0;
					ComponentType componentType = (ComponentType)property.Type;

					foreach (Column column in columns)
						ValidateStringValue(column, sourceObject, properties[componentType.PropertyNames[i++]]);
				}
				else
				{
					//There is only one column, but we have to iterate.
					foreach (Column column in columns)
						ValidateStringValue(column, obj, pclassProperties[property.Name]);
				}
			}
		}
	}
}
