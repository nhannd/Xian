using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    [ExtensionPoint]
    public class PersistentStoreExtensionPoint : ExtensionPoint<IPersistentStore>
    {
    }


    public class PersistentStoreRegistry
    {
        private static IPersistentStore _defaultStore;

        public static IPersistentStore GetDefaultStore()
        {
            if (_defaultStore == null)
            {
                // for now, just look for a single extension and treat it as the "default" store
                // in future, there could conceivably be a number of different persistent stores,
                // in which case we will need to use a different mechanism (config file or something)
                _defaultStore = (IPersistentStore)(new PersistentStoreExtensionPoint()).CreateExtension();
                _defaultStore.Initialize();
            }
            return _defaultStore;
        }
    }
}
