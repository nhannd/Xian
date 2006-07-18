using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    public class PersistentStoreExtensionPoint : ExtensionPoint<IPersistentStore>
    {
    }

    public class Session : ISession
    {
        private static ISession _current;


        public static ISession Current
        {
            get { return _current; }
            set { _current = value; }
        }


        private IPersistentStore _dataStore;
        private IServiceManager _serviceManager;
        private TransactionMonitor _transactionMonitor;

        internal Session()
        {
            // initialize the data store now, rather than deferring it
            // (the session is pretty useless without a data-store)
            PersistentStoreExtensionPoint xp = new PersistentStoreExtensionPoint();
            _dataStore = (IPersistentStore)xp.CreateExtension();
            _dataStore.Initialize();
        }

        protected IPersistentStore DataStore
        {
            get
            {
                return _dataStore;
            }
        }

        public IServiceManager ServiceManager
        {
            get
            {
                if (_serviceManager == null)
                {
                    _serviceManager = new ServiceManager(this);
                }
                return _serviceManager;
            }
        }

        public ITransactionMonitor TransactionMonitor
        {
            get
            {
                if (_transactionMonitor == null)
                {
                    _transactionMonitor = new TransactionMonitor(this);
                }
                return _transactionMonitor;
            }
        }

        public IReadContext GetReadContext()
        {
            return this.DataStore.GetReadContext();
        }

        public IUpdateContext GetUpdateContext()
        {
            return this.DataStore.GetUpdateContext();
        }
    }
}
