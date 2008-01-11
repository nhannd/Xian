using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Defines an extension point for configuration stores.  Used by <see cref="ConfigurationStoreFactory"/>
    /// to create configuration store instances.
    /// </summary>
    [ExtensionPoint]
    public class ConfigurationStoreExtensionPoint : ExtensionPoint<IConfigurationStore>
    {
    }

    /// <summary>
    /// Factory class for creating instances of <see cref="IConfigurationStore"/>.
    /// </summary>
    public static class ConfigurationStoreFactory
    {
        /// <summary>
        /// Obtains the default configuration store, if one exists.  Otherwise an exception will be thrown.
        /// </summary>
        /// <exception cref="NotSupportedException">Indicates that there is no default configuration store.</exception>
        /// <returns></returns>
        public static IConfigurationStore GetDefaultStore()
        {
            return (IConfigurationStore)new ConfigurationStoreExtensionPoint().CreateExtension();
        }
    }
}
