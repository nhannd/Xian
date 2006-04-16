using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark a class as defining an extension point.
    /// </summary>
    /// <remarks>
    /// Use this attribute to mark a class as defining an extension point.  This attribute must only be
    /// applied to subclasses of <see cref="ExtensionPoint">.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
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
        /// A friendly name for the extension point.  This is optional and may be supplied as a named parameter.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// A friendly description for the extension point.  This is optional and may be supplied as a named parameter.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
