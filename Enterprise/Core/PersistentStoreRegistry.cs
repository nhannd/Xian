#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	[ExtensionPoint]
	public class PersistentStoreExtensionPoint : ExtensionPoint<IPersistentStore>
	{
	}

	/// <summary>
	/// Static class for obtaining singleton persistent store objects.
	/// </summary>
	public static class PersistentStoreRegistry
	{
		private static readonly object _syncLock = new object();
		private static volatile IPersistentStore _defaultStore;

		/// <summary>
		/// Gets the default persistent store, creating it if not yet created.
		/// </summary>
		/// <remarks>
		/// This method can safely be called from multiple threads.
		/// </remarks>
		/// <returns></returns>
		public static IPersistentStore GetDefaultStore()
		{
			if (_defaultStore == null)
			{
				lock (_syncLock)
				{
					if(_defaultStore == null)
					{
						// for now, just look for a single extension and treat it as the "default" store
						// in future, there could conceivably be a number of different persistent stores,
						// in which case we will need to use a different mechanism (config file or something)
						var store = (IPersistentStore)(new PersistentStoreExtensionPoint()).CreateExtension();
						store.Initialize();

						// assign to static variable here, after initialization is complete
						_defaultStore = store;
					}
				}
			}
			return _defaultStore;
		}
	}
}
