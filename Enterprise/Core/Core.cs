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

    public static class Core
    {
        private static IServiceManager _serviceManager;
        private static ITransactionNotifier _transactionNotifier;
        private static IPersistentStore _dataStore;

        public static IPersistentStore DataStore
        {
            get
            {
                if (_dataStore == null)
                {
                    PersistentStoreExtensionPoint xp = new PersistentStoreExtensionPoint();
                    _dataStore = (IPersistentStore)xp.CreateExtension();
                    _dataStore.Initialize();
                    _dataStore.SetTransactionNotifier(TransactionNotifier);

                }
                return _dataStore;
            }
        }

        public static IServiceManager ServiceManager
        {
            get
            {
                if (_serviceManager == null)
                {
                    _serviceManager = new ServiceManager();
                }
                return _serviceManager;
            }
        }

        public static ITransactionNotifier TransactionNotifier
        {
            get
            {
                if (_transactionNotifier == null)
                {
                    _transactionNotifier = new TransactionNotifier();
                }
                return _transactionNotifier;
            }
        }
    }
}
