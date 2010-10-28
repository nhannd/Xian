#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class ExtensionOfAttribute: Attribute
    {
        private readonly Type _extensionPointClass;
        private string _name;
        private string _description;
    	private bool _enabled = true;	// extensions are enabled by default

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="extensionPointClass">The type of the extension point class which the target class extends.</param>
        public ExtensionOfAttribute(Type extensionPointClass)
        {
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
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// A friendly description for the extension.
        /// </summary>
        /// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>
		/// The default enablement of the extension.
		/// </summary>
    	public bool Enabled
    	{
			get { return _enabled; }
			set { _enabled = value; }
    	}
    }
}
