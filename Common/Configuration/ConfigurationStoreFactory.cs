#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
