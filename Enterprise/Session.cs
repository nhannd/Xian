using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public abstract class Session
    {
        private static Session _current;


        public static Session Current
        {
            get { return _current; }
            set { _current = value; }
        }


        private ServiceManager _serviceManager;
        private TransactionMonitor _transactionMonitor;

        protected abstract IPersistentStore DataStore { get; }

        public ServiceManager ServiceManager
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
