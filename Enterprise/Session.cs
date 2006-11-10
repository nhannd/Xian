using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    public class PersistentStoreExtensionPoint : ExtensionPoint<IPersistentStore>
    {
    }

    /// <summary>
    /// Implementation of <see cref="ISession"/>
    /// </summary>
    internal class Session : ISession
    {
        private static Session _current;


        internal static Session Current
        {
            get { return _current; }
            set { _current = value; }
        }


        private IServiceManager _serviceManager;
        private ITransactionNotifier _transactionNotifier;
        private IPersistentStore _dataStore;

        internal Session()
        {
            // TODO this stuff doesn't really belong here, does it?
            PersistentStoreExtensionPoint xp = new PersistentStoreExtensionPoint();
            _dataStore = (IPersistentStore)xp.CreateExtension();
            _dataStore.Initialize();
            _dataStore.SetTransactionNotifier(this.TransactionNotifier);
        }

        /// <summary>
        /// TODO this shouldn't really be a session property, should it?
        /// </summary>
        public IPersistentStore DataStore
        {
            get { return _dataStore; }
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

        public ITransactionNotifier TransactionNotifier
        {
            get
            {
                if (_transactionNotifier == null)
                {
                    _transactionNotifier = new TransactionNotifier(this);
                }
                return _transactionNotifier;
            }
        }
    }
}
