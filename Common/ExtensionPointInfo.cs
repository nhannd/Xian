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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Describes an extension point.  
    /// </summary>
    /// <remarks>
    /// Instances of this class are constructed by the framework when it processes
    /// plugins looking for extension points.
    /// </remarks>
    public class ExtensionPointInfo : IBrowsable
    {
        private Type _extensionPointClass;
        private Type _extensionInterface;
        private string _name;
        private string _description;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal ExtensionPointInfo(Type extensionPointClass, Type extensionInterface, string name, string description)
        {
            _extensionPointClass = extensionPointClass;
            _extensionInterface = extensionInterface;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// Gets the class that defines the extension point.
        /// </summary>
        public Type ExtensionPointClass
        {
            get { return _extensionPointClass; }
        }

        /// <summary>
        /// Gets the interface that an extension must implement.
        /// </summary>
        public Type ExtensionInteface
        {
            get { return _extensionInterface; }
        }

        /// <summary>
        /// Computes and returns a list of the installed extensions to this point,
        /// including disabled extensions.
        /// </summary>
        /// <returns></returns>
        public IList<ExtensionInfo> ListExtensions()
        {
            return CollectionUtils.Select(Platform.PluginManager.Extensions,
                delegate(ExtensionInfo ext) { return ext.PointExtended == _extensionPointClass; });
        }

        #region IBrowsable Members

        /// <summary>
        /// Friendly name of the extension point, if one exists, otherwise null.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// A friendly description of the extension point, if one exists, otherwise null.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Formal name of the extension, which is the fully qualified name of the extension point class.
        /// </summary>
        public string FormalName
        {
            get { return _extensionPointClass.FullName; }
        }

        #endregion
    }
}
