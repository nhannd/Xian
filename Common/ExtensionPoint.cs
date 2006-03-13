using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Describes an extension point.  
    /// </summary>
    /// <remarks>
    /// Instances of this class are constructed by the framework when it processes
    /// plugins looking for extension points. An extension point is any class within a plugin that is marked
    /// with the <see cref="ExtensionPointAttribute"/> attribute.
    /// </remarks>
    public class ExtensionPoint
    {
        private Type _extensibleType;

        private string _name;
        private string _description;

        internal ExtensionPoint(Type extensibleType, string name, string description)
        {
            _extensibleType = extensibleType;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// The type of the interface that this extension point is defined on.
        /// </summary>
        public Type ExtensibleType
        {
            get { return _extensibleType; }
        }

        /// <summary>
        /// Friendly name of this extension point, if one exists, otherwise null;
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// A friendly description of this extension point, if one exists, otherwise null.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
