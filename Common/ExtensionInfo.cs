#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Describes an extension.  
	/// </summary>
	/// <remarks>
	/// Instances of this class are constructed by the framework when it processes
	/// plugins looking for extensions.
	/// </remarks>
	public class ExtensionInfo : IBrowsable
	{
		private readonly Type _extensionClass;
		private readonly Type _pointExtended;
		private readonly string _name;
		private readonly string _description;
		private readonly bool _enabled;
		private readonly string _featureToken;

		public ExtensionInfo(Type extensionClass, Type pointExtended, string name, string description, bool enabled)
			: this(extensionClass, pointExtended, name, description, enabled, null) {}

		public ExtensionInfo(Type extensionClass, Type pointExtended, string name, string description, bool enabled, string featureToken)
		{
			_extensionClass = extensionClass;
			_pointExtended = pointExtended;
			_name = name;
			_description = description;
			_enabled = enabled;
			_featureToken = featureToken;
		}

		/// <summary>
		/// Gets the type that implements the extension.
		/// </summary>
		public Type ExtensionClass
		{
			get { return _extensionClass; }
		}

		/// <summary>
		/// Gets the extension point type which this extension extends.
		/// </summary>
		public Type PointExtended
		{
			get { return _pointExtended; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this extension is enabled by application configuration.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this extension is authorized by application licensing.
		/// </summary>
		public bool Authorized
		{
			get { return string.IsNullOrEmpty(_featureToken) || LicenseInformation.IsFeatureAuthorized(_featureToken); }
		}

		/// <summary>
		/// Gets the feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken
		{
			get { return _featureToken; }
		}

		#region IBrowsable Members

		/// <summary>
		/// Gets a friendly name of this extension, if one exists, otherwise null.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a friendly description of this extension, if one exists, otherwise null.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		/// <summary>
		/// Gets the formal name of this extension, which is the fully qualified name of the extension class.
		/// </summary>
		public string FormalName
		{
			get { return _extensionClass.FullName; }
		}

		#endregion
	}
}