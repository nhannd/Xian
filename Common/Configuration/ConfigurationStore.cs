#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Defines an extension point for configuration stores.  Used by <see cref="ConfigurationStore"/>
	/// to create configuration store instances.
	/// </summary>
	[ExtensionPoint]
	public class ConfigurationStoreExtensionPoint : ExtensionPoint<IConfigurationStore>
	{
	}

	/// <summary>
	/// Factory class for creating instances of <see cref="IConfigurationStore"/>.
	/// </summary>
	public static class ConfigurationStore
	{
		static ConfigurationStore()
		{
			IsSupported = new ConfigurationStoreExtensionPoint().ListExtensions().Length > 0;
		}

		/// <summary>
		/// Gets a value indicating whether the configuration store is supported under the current deployment.
		/// </summary>
		public static bool IsSupported { get; private set; }

		/// <summary>
		/// Obtains an instance of the configuration store, if supported.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Indicates that there is no configuration store extension.</exception>
		public static IConfigurationStore Create()
		{
			return (IConfigurationStore)new ConfigurationStoreExtensionPoint().CreateExtension();
		}
	}
}
