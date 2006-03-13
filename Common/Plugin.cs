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
    public class Plugin
    {
        /// <summary>
        /// Internal utility method used by the framework to discover the extensions declared in an assembly.
        /// </summary>
        /// <param name="asm">The assembly to inspect</param>
        /// <returns>An array of extensions</returns>
        internal static Extension[] DiscoverExtensions(Assembly asm)
        {
            List<Extension> extensionList = new List<Extension>();
            foreach (Type type in asm.GetTypes())
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(type);
                foreach (Attribute attr in attrs)
                {
                    if (attr is ExtensionOfAttribute)
                    {
                        ExtensionOfAttribute a = (ExtensionOfAttribute)attr;
                        extensionList.Add(new Extension(type, a.ExtensionOfType, a.Name, a.Description));
                    }
                }
            }
            return extensionList.ToArray();
        }

        /// <summary>
        /// Internal utility method used by the framework to discover the extension points declared in an assembly.
        /// </summary>
        /// <param name="asm">The assembly to inspect</param>
        /// <returns>An array of extension points</returns>
        internal static ExtensionPoint[] DiscoverExtensionPoints(Assembly asm)
        {
            List<ExtensionPoint> extensionPointList = new List<ExtensionPoint>();
            foreach (Type type in asm.GetTypes())
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(type);
                foreach (Attribute attr in attrs)
                {
                    if (attr is ExtensionPointAttribute)
                    {
                        ExtensionPointAttribute a = (ExtensionPointAttribute)attr;
                        extensionPointList.Add(new ExtensionPoint(type, a.Name, a.Description));
                    }
                }
            }
            return extensionPointList.ToArray();
        }

        
        private string _name;
        private string _description;
        private Assembly _assembly;

        private ExtensionPoint[] _extensionPoints;
        private Extension[] _extensions;

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
        public Extension[] Extensions
        {
            get { return _extensions; }
        }

        /// <summary>
        /// The set of extension points defined in this plugin.
        /// </summary>
        public ExtensionPoint[] ExtensionPoints
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

        /// <summary>
        /// Creates an instance of the extension of the specified type, searching only this plugin
        /// for extensions.  If more than one extension is found, only the first one is created.  If no
        /// extensions are found, a <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An instance of the requested extension.</returns>
        public object CreateExtension(Type extensionOfType)
        {
            Platform.CheckForNullReference(extensionOfType, "extensionOfType");

            return ExtensionLoader.CreateExtension(_extensions, extensionOfType, null);
        }

        /// <summary>
        /// Creates an instance of the extension of the specified type, searching only this plugin
        /// for extensions.  Only extensions matching the specified filter are considered.
        /// If more than one extension is found, only the first one is created.  If no
        /// extensions are found, a <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <param name="filter">The filter provides additional criteria to select extensions</param>
        /// <returns>An instance of the requested extension.</returns>
        public object CreateExtension(Type extensionOfType, ExtensionFilter filter)
        {
            Platform.CheckForNullReference(extensionOfType, "extensionOfType");
            Platform.CheckForNullReference(filter, "filter");

            return ExtensionLoader.CreateExtension(_extensions, extensionOfType, filter);
        }

        /// <summary>
        /// Creates an instance of each extension of the specified type, searching only this plugin
        /// for extensions.  If no extensions are found, the returned array is empty.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An array containing one instance of each extension that was created.</returns>
        public object[] CreateExtensions(Type extensionOfType)
        {
            Platform.CheckForNullReference(extensionOfType, "extensionOfType");
            return ExtensionLoader.CreateExtensions(_extensions, extensionOfType, null);
        }

        /// <summary>
        /// Creates an instance of each extension of the specified type, searching only this plugin
        /// for extensions.  If no extensions are found, the returned array is empty.
        /// </summary>
        /// <param name="filter">The filter provides additional criteria to select extensions</param>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An array containing one instance of each extension that was created.</returns>
        public object[] CreateExtensions(Type extensionOfType, ExtensionFilter filter)
        {
            Platform.CheckForNullReference(extensionOfType, "extensionOfType");
            Platform.CheckForNullReference(filter, "filter");

            return ExtensionLoader.CreateExtensions(_extensions, extensionOfType, filter);
        }
    }
}
