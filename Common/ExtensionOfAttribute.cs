using System;
using System.Collections.Generic;
using System.Text;

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
        private Type _extensionPointClass;
        private string _name;
        private string _description;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="extensionPointClass">The type of the extension point class which the target class extends</param>
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
