using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// The default implementation of <see cref="IExtensionFactory"/> that creates extensions from
    /// the set of plugins discovered at runtime.
    /// </summary>
    internal class DefaultExtensionFactory : IExtensionFactory
    {
        internal DefaultExtensionFactory()
        {
        }

        #region IExtensionFactory Members

        public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            Type extensionPointClass = extensionPoint.GetType();

            // get subset of applicable extensions
            ExtensionInfo[] extensions = ListExtensions(extensionPoint, filter);

            // attempt to instantiate the extension classes
            List<object> createdObjects = new List<object>();
            foreach (ExtensionInfo extension in extensions)
            {
                if (justOne && createdObjects.Count > 0)
                    break;

                // is the extension a concrete class?
                if (!IsConcreteClass(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionMustBeConcreteClass,
                        extension.ExtensionClass.FullName);
                    continue;
                }

                // does the extension implement the required interface?
                if (!extensionPoint.InterfaceType.IsAssignableFrom(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionDoesNotImplementRequiredInterface,
                        extension.ExtensionClass.FullName,
                        extensionPoint.InterfaceType);

                    continue;
                }

                try
                {
                    // instantiate
                    object o = Activator.CreateInstance(extension.ExtensionClass);
                    createdObjects.Add(o);
                }
                catch (Exception e)
                {
                    // instantiation failed
                    Platform.Log(LogLevel.Error, e);
                }
            }

            return createdObjects.ToArray();
        }

        public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            Type extensionPointClass = extensionPoint.GetType();

            List<ExtensionInfo> extensions = new List<ExtensionInfo>();
            foreach (ExtensionInfo extension in Platform.PluginManager.Extensions)
            {
                if (extension.PointExtended == extensionPointClass
                    && (filter == null || filter.Test(extension)))
                {
                    extensions.Add(extension);
                }
            }
            return extensions.ToArray();
        }

        private bool IsConcreteClass(Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        #endregion
    }
}
