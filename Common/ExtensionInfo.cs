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
    public class ExtensionInfo : IBrowsable
    {
        private Type _extensionClass;
        private Type _extensionPointClass;
        private string _name;
        private string _description;

        internal ExtensionInfo(Type extensionClass, Type extensionPointClass, string name, string description)
        {
            _extensionClass = extensionClass;
            _extensionPointClass = extensionPointClass;
            _name = name;
            _description = description;
        }

        public Type ExtensionClass
        {
            get { return _extensionClass; }
        }

        public Type ExtensionPointClass
        {
            get { return _extensionPointClass; }
        }

        #region IBrowsable Members

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

        /// <summary>
        /// Formal name of this extension.
        /// </summary>
        public string FormalName
        {
            get { return _extensionClass.FullName; }
        }

        #endregion
    }
}
