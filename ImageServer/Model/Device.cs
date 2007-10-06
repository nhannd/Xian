using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Model
{
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public class Device : ServerEntity
    {
        #region Constructors
        public Device()
            : base("Device")
        {
        }
        #endregion

        #region Private Members
        private String _aeTitle;
        private String _description;
        private String _ipAddress;
        private ServerEntityKey _serverPartitionKey;    
        private bool _active;
        private bool _dhcp;
        private int _port;
        private ServerPartition _serverPartition;
        #endregion

        #region Public Properties
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool Dhcp
        {
            get { return _dhcp; }
            set { _dhcp = value; }
        }

        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }

        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public String AeTitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }

        public String IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public ServerPartition ServerPartition
        {
            get
            {
                if (_serverPartition == null)
                    _serverPartition = ServerPartition.Load(_serverPartitionKey);
                return _serverPartition;
            }
        }
        #endregion

        #region Static Methods
        static public Device Load(ServerEntityKey key)
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            ISelectDevice broker = read.GetBroker<ISelectDevice>();
            Device theDevice = broker.Load(key);
            read.Dispose();
            return theDevice;
        }
        #endregion
    }
}
