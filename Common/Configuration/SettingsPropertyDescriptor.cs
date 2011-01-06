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
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Runtime.Serialization;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Describes a settings property.
	/// </summary>
	/// <remarks>
	/// A settings property is a single property belonging to a settings group.
	/// </remarks>
	[DataContract]
	public class SettingsPropertyDescriptor
	{
		/// <summary>
		/// Returns a list of <see cref="SettingsPropertyDescriptor"/> objects describing each property belonging
		/// to a settings group.
		/// </summary>
		public static List<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
		{
			Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName);

			return CollectionUtils.Map(SettingsClassMetaDataReader.GetSettingsProperties(settingsClass),
									   (PropertyInfo property) => new SettingsPropertyDescriptor(property));
		}


		private string _name;
		private string _typeName;
		private string _description;
		private SettingScope _scope;
		private string _defaultValue;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SettingsPropertyDescriptor(PropertyInfo property)
			: this(SettingsClassMetaDataReader.GetName(property),
				SettingsClassMetaDataReader.GetType(property).FullName,
				SettingsClassMetaDataReader.GetDescription(property),
				SettingsClassMetaDataReader.GetScope(property),
				SettingsClassMetaDataReader.GetDefaultValue(property))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsPropertyDescriptor(string name, string typeName, string description, SettingScope scope, string defaultValue)
		{
			_name = name;
			_typeName = typeName;
			_description = description;
			_scope = scope;
			_defaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the name of the type of the property.
		/// </summary>
		[DataMember]
		public string TypeName
		{
			get { return _typeName; }
			private set { _typeName = value; }
		}

		/// <summary>
		/// Gets the description of the property.
		/// </summary>
		[DataMember]
		public string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		/// <summary>
		/// Gets the scope of the property.
		/// </summary>
		[DataMember]
		public SettingScope Scope
		{
			get { return _scope; }
			private set { _scope = value; }
		}

		/// <summary>
		/// Gets the serialized default value of the property.
		/// </summary>
		[DataMember]
		public string DefaultValue
		{
			get { return _defaultValue; }
			private set { _defaultValue = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", Name, Scope, TypeName);
		}
	}
}
