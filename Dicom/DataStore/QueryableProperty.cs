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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Dicom.DataStore
{
	internal class QueryablePropertyInfo
	{
		private readonly QueryablePropertyAttribute _attribute;

		public QueryablePropertyInfo(QueryablePropertyAttribute attribute, PropertyInfo property)
		{
			_attribute = attribute;
			Property = property;

			ColumnName = String.Format("{0}_", property.Name);

			ReturnProperty = property;
			if (attribute.ReturnProperty != null)
				ReturnProperty = property.DeclaringType.GetProperty(attribute.ReturnProperty) ?? Property;

			Type convertType = ReturnProperty.PropertyType;
			if (convertType.IsArray)
				convertType = convertType.GetElementType();

			ReturnPropertyConverter = TypeDescriptor.GetConverter(convertType);
			Debug.Assert(ReturnPropertyConverter.CanConvertFrom(typeof (string)) && ReturnPropertyConverter.CanConvertTo(typeof (string)),
				"The property type must be convertible to/from a string.");

			AllowListMatching = Path.ValueRepresentation == DicomVr.UIvr || Path.Equals(DicomTags.ModalitiesInStudy);
		}

		public readonly PropertyInfo Property;
		public readonly string ColumnName;

		public readonly TypeConverter ReturnPropertyConverter;

		public DicomTagPath Path
		{
			get { return _attribute.Path; }
		}

		public bool IsHigherLevelUnique
		{
			get { return _attribute.IsHigherLevelUnique; }	
		}

		public bool IsUnique
		{
			get { return _attribute.IsUnique; }
		}

		public bool IsRequired
		{
			get { return _attribute.IsRequired; }
		}

		public bool PostFilterOnly
		{
			get { return _attribute.PostFilterOnly; }
		}

		public readonly bool AllowListMatching;

		public readonly PropertyInfo ReturnProperty;

		public override string ToString()
		{
			return Path.ToString();
		}
	}

	internal class QueryablePropertyAttribute : Attribute
	{
		public QueryablePropertyAttribute(params DicomTag[] tags)
		{
			Path = new DicomTagPath(tags);
		}

		public QueryablePropertyAttribute(params uint[] tags)
		{
			Path = new DicomTagPath(tags);
		}

		public DicomTagPath Path;
		public bool IsHigherLevelUnique = false;
		public bool IsUnique = false;
		public bool IsRequired = false;
		public bool PostFilterOnly = false;

		public string ReturnProperty = null;
	}

	internal static class QueryableProperties<T>
	{
		private static readonly Dictionary<DicomTagPath, QueryablePropertyInfo> _dictionary;

		static QueryableProperties()
		{
			_dictionary = new Dictionary<DicomTagPath, QueryablePropertyInfo>();

			foreach (PropertyInfo property in typeof(T).GetProperties())
			{
				foreach (QueryablePropertyAttribute attribute in property.GetCustomAttributes(typeof(QueryablePropertyAttribute), false))
				{
					_dictionary[attribute.Path] = new QueryablePropertyInfo(attribute, property);
				}
			}
		}

		public static IEnumerable<QueryablePropertyInfo> GetProperties()
		{
			return _dictionary.Values;
		}

		public static QueryablePropertyInfo GetProperty(DicomTagPath path)
		{
				if (!_dictionary.ContainsKey(path))
					return null;

				return _dictionary[path];
		}
	}
}