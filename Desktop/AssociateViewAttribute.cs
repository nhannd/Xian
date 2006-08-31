using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Associates a view extension point with a "model" class.  The model class may be
    /// any class that participates in a model-view relationship and defines an associated
    /// view extension point.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AssociateViewAttribute : Attribute
    {
        private Type _viewExtensionPointType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewExtensionPointType">The view extension point class</param>
        public AssociateViewAttribute(Type viewExtensionPointType)
        {
            _viewExtensionPointType = viewExtensionPointType;
        }

        /// <summary>
        /// Gets the view extension point class
        /// </summary>
        public Type ViewExtensionPointType
        {
            get { return _viewExtensionPointType; }
        }
    }
}
