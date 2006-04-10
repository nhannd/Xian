using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Describes a plugin, and provides properties for querying the extension points and extensions defined
    /// in the plugin, and methods for creating the extensions.
    /// </summary>
    public class Plugin : IBrowsable
    {
        /// <summary>
        /// Internal utility method used by the framework to discover the extensions declared in an assembly.
        /// </summary>
        /// <param name="asm">The assembly to inspect</param>
        /// <returns>An array of extensions</returns>
        internal static ExtensionInfo[] DiscoverExtensions(Assembly asm)
        {
            List<ExtensionInfo> extensionList = new List<ExtensionInfo>();
            foreach (Type type in asm.GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(ExtensionOfAttribute), false);
                foreach (ExtensionOfAttribute a in attrs)
                {
                    extensionList.Add(new ExtensionInfo(type, a.ExtensionPointClass, a.Name, a.Description));
                }
            }
            return extensionList.ToArray();
        }

        /// <summary>
        /// Internal utility method used by the framework to discover the extension points declared in an assembly.
        /// </summary>
        /// <param name="asm">The assembly to inspect</param>
        /// <returns>An array of extension points</returns>
        internal static ExtensionPointInfo[] DiscoverExtensionPoints(Assembly asm)
        {
            List<ExtensionPointInfo> extensionPointList = new List<ExtensionPointInfo>();
            foreach (Type type in asm.GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(ExtensionPointAttribute), false);
                foreach (ExtensionPointAttribute a in attrs)
                {
                    extensionPointList.Add(new ExtensionPointInfo(type, a.Name, a.Description));
                }
            }
            return extensionPointList.ToArray();
        }

        
        private string _name;
        private string _description;
        private Assembly _assembly;

        private ExtensionPointInfo[] _extensionPoints;
        private ExtensionInfo[] _extensions;

        /// <summary>
        /// Default constructor, used internally by the framework.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        internal Plugin(Assembly assembly, string name, string description)
        {
            _name = name;
            _description = description;
            _assembly = assembly;

            _extensionPoints = DiscoverExtensionPoints(assembly);
            _extensions = DiscoverExtensions(assembly);
        }

        /// <summary>
        /// The set of extensions defined in this plugin.
        /// </summary>
        public ExtensionInfo[] Extensions
        {
            get { return _extensions; }
        }

        /// <summary>
        /// The set of extension points defined in this plugin.
        /// </summary>
        public ExtensionPointInfo[] ExtensionPoints
        {
            get { return _extensionPoints; }
        }

        /// <summary>
        /// The assembly that implements this plugin.
        /// </summary>
        public Assembly Assembly
        {
            get { return _assembly; }
        }

        #region IBrowsable Members

        public string FormalName
        {
            get { return Assembly.FullName; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        #endregion
    }
}
