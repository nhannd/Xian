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

        /// <summary>
        /// Internal constructor.
        /// </summary>
		public ExtensionInfo(Type extensionClass, Type pointExtended, string name, string description, bool enabled)
        {
            _extensionClass = extensionClass;
            _pointExtended = pointExtended;
            _name = name;
            _description = description;
        	_enabled = enabled;
        }

        /// <summary>
        /// The class that implements the extension.
        /// </summary>
        public Type ExtensionClass
        {
            get { return _extensionClass; }
        }

        /// <summary>
        /// The class that defines the extension point which this extension extends.
        /// </summary>
        public Type PointExtended
        {
            get { return _pointExtended; }
        }

		/// <summary>
		/// Gets a value indicatign whether this extension is enabled.
		/// </summary>
    	public bool Enabled
    	{
			get { return _enabled; }
    	}

        #region IBrowsable Members

        /// <summary>
        /// Friendly name of this extension, if one exists, otherwise null.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// A friendly description of this extension, if one exists, otherwise null.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Formal name of this extension, which is the fully qualified name of the extension class.
        /// </summary>
        public string FormalName
        {
            get { return _extensionClass.FullName; }
        }

        #endregion
    }
}
