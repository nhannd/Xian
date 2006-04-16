using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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
        private string _name;
        private string _description;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal ExtensionPointInfo(Type extensionPointClass, string name, string description)
        {
            _extensionPointClass = extensionPointClass;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// The class that defines the extension point.
        /// </summary>
        public Type ExtensionPointClass
        {
            get { return _extensionPointClass; }
        }

        #region IBrowsable Members

        /// <summary>
        /// Friendly name of this extension point, if one exists, otherwise null.
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

        /// <summary>
        /// Formal name of this extension, which is the fully qualified name of the extension point class.
        /// </summary>
        public string FormalName
        {
            get { return _extensionPointClass.FullName; }
        }

        #endregion
    }
}
