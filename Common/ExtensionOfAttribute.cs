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
	/// Attribute used to mark a class as being an extension of the specified extension point class.
	/// </summary>
	/// <remarks>
	/// Use this attribute to mark a class as being an extension of the specified extension point,
	/// specified by the <see cref="Type" /> of the extension point class.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ExtensionOfAttribute : Attribute
	{
		private readonly Type _extensionPointClass;

		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="extensionPointClass">The type of the extension point class which the target class extends.</param>
		public ExtensionOfAttribute(Type extensionPointClass)
		{
			Enabled = true;
			_extensionPointClass = extensionPointClass;
		}

		/// <summary>
		/// The class that defines the extension point which this extension extends.
		/// </summary>
		public Type ExtensionPointClass
		{
			get { return _extensionPointClass; }
		}

		/// <summary>
		/// A friendly name for the extension.
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// A friendly description for the extension.
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		/// The default enablement of the extension.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken { get; set; }
	}
}