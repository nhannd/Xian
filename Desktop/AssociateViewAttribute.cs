#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Associates a view extension point with a "model" class.
    /// </summary>
    /// <remarks>
	/// The model class may be any class that participates in a model-view 
	/// relationship and defines an associated view extension point.
	/// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AssociateViewAttribute : Attribute
    {
        private Type _viewExtensionPointType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="viewExtensionPointType">The view extension point class.</param>
        public AssociateViewAttribute(Type viewExtensionPointType)
        {
            _viewExtensionPointType = viewExtensionPointType;
        }

        /// <summary>
        /// Gets the view extension point class.
        /// </summary>
        public Type ViewExtensionPointType
        {
            get { return _viewExtensionPointType; }
        }
    }
}
