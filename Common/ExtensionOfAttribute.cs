using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark a class as being an extension of the specified type, which must be
    /// an interface, and must have been marked with the <see cref="ExtensionOfAttribute"/> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class ExtensionOfAttribute: Attribute
    {
        private Type _extensionPointClass;
        private string _name;
        private string _description;

        public ExtensionOfAttribute(Type extensionPointClass)
        {
            _extensionPointClass = extensionPointClass;
        }

        /// <summary>
        /// The type of interface on which the extension point is defined.
        /// </summary>
        public Type ExtensionPointClass
        {
            get { return _extensionPointClass; }
        }

        /// <summary>
        /// A friendly name for the extension.  This is optional and may be supplied as a named parameter.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// A friendly description for the extension.  This is optional and may be supplied as a named parameter.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
