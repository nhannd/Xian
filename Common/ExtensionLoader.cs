using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// A utility class that provides a set of static methods used to create instances
    /// of extension classes at runtime.  This class is for internal use only.  Clients that
    /// want to instantiate extension classes should use <see cref="Platform.CreateExtensions"/>
    /// or <see cref="Plugin.CreateExtensions"/> instead.
    /// </summary>
    internal class ExtensionLoader
    {
        /// <summary>
        /// Creates one instance of the first extension type from the specified set of extensions that is
        /// an extension of the extensionOfType and matches the specified filter.  If no such extension is found,
        /// the method will throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="extensionSet">The set of extensions to consider</param>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <param name="filter">A filter which is used exclude extensions from the set based on the filter criteria</param>
        /// <returns></returns>
        internal static object CreateExtension(Extension[] extensionSet, Type extensionOfType, ExtensionFilter filter)
        {
            object[] res = CreateExtensions(extensionSet, extensionOfType, filter, true);
            return AtLeastOne(res);
        }

        /// <summary>
        /// Creates one instance of each extension type from the specified set of extensions that is
        /// an extension of the extensionOfType and matches the specified filter.
        /// </summary>
        /// <param name="extensionSet">The set of extensions to consider</param>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <param name="filter">A filter which is used exclude extensions from the set based on the filter criteria</param>
        /// <returns></returns>
        internal static object[] CreateExtensions(Extension[] extensionSet, Type extensionOfType, ExtensionFilter filter)
        {
            return CreateExtensions(extensionSet, extensionOfType, filter, false);
        }

        // Private helper method
        private static object[] CreateExtensions(Extension[] extensionSet, Type extensionOfType, ExtensionFilter filter, bool firstOnly)
        {
            List<object> createdObjects = new List<object>();
            foreach (Extension extension in extensionSet)
            {
                if (firstOnly && createdObjects.Count > 0)
                {
                    break;
                }

                if (extension.ExtensionOfType == extensionOfType
                    && IsConcreteClass(extension.ExtensionType)
                    && (filter == null || filter.Test(extension)))
                {
                    try
                    {
                        if (!extension.ExtensionOfType.IsAssignableFrom(extension.ExtensionType))
                        {
                            // TODO better error message
                            throw new Exception("The extension does not implement the required interface");
                        }
                        
                        object o = Activator.CreateInstance(extension.ExtensionType);
                        createdObjects.Add(o);
                    }
                    catch (Exception e)
                    {
                        //TODO appropriate handling
                        Platform.HandleException(e);
                    }

                }
            }
            return createdObjects.ToArray();
        }

        private static bool IsConcreteClass(Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        private static object AtLeastOne(object[] objs)
        {
            if (objs.Length > 0)
            {
                return objs[0];
            }
            else
            {
                // since no extensions were created, the extension point is effectively
                // "not supported"
                // TODO detailed error message
                throw new NotSupportedException();
            }
        }
    }
}
