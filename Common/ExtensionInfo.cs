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
    /// plugins looking for extensions.
    /// </remarks>
    public class ExtensionInfo : IBrowsable
    {
        private Type _extensionClass;
        private Type _pointExtended;
        private string _name;
        private string _description;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        public ExtensionInfo(Type extensionClass, Type pointExtended, string name, string description)
        {
            _extensionClass = extensionClass;
            _pointExtended = pointExtended;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// The class that implements the extension.
        /// </summary>
        public Type ExtensionClass
        {
            get { return _extensionClass; }
        }

        /// <summary>
        /// The class that defines the extension point which this extension extends.
        /// </summary>
        public Type PointExtended
        {
            get { return _pointExtended; }
        }

        #region IBrowsable Members

        /// <summary>
        /// Friendly name of this extension, if one exists, otherwise null.
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
        /// Formal name of this extension, which is the fully qualified name of the extension class.
        /// </summary>
        public string FormalName
        {
            get { return _extensionClass.FullName; }
        }

        #endregion
    }
}
