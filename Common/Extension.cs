using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Describes an extension.  
    /// </summary>
    /// <remarks>
    /// Instances of this class are constructed by the framework when it processes
    /// plugins looking for extensions. An extension is any class within a plugin that is marked
    /// with the <see cref="ExtensionOfAttribute"/> attribute.
    /// </remarks>
    public class Extension
    {
        private Type _extensionType;
        private Type _extensionPointType;

        private string _name;
        private string _description;

        internal Extension(Type extensionType, Type extensionPointType, string name, string description)
        {
            _extensionType = extensionType;
            _extensionPointType = extensionPointType;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// The type of this extension.
        /// </summary>
        public Type ExtensionType
        {
            get { return _extensionType; }
        }

        /// <summary>
        /// The type of the <see cref="ExtensionPoint"/> interface that is extended.
        /// </summary>
        public Type ExtensionOfType
        {
            get { return _extensionPointType; }
        }

        /// <summary>
        /// Friendly name of this extension, if one exists, otherwise null;
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
    }
}
