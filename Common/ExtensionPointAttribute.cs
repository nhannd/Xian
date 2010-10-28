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
    /// Attribute used to mark a class as defining an extension point.
    /// </summary>
    /// <remarks>
    /// Use this attribute to mark a class as defining an extension point.  This attribute must only be
    /// applied to subclasses of <see cref="ExtensionPoint" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ExtensionPointAttribute : Attribute
    {
        private string _name;
        private string _description;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public ExtensionPointAttribute()
        {
        }

        /// <summary>
        /// A friendly name for the extension point.  
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
		/// A friendly description for the extension point.  
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
