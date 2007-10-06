using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Model
{
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public class ServerPartition : ServerEntity
    {
        #region Constructors
        public ServerPartition()
            : base("ServerPartition")
        {
        }
        #endregion

        #region Private Members
        private bool _enabled;
        private String _description;
        private string _aeTitle;
        private int _port;
        private string _partitionFolder;
        ServerEntityKey _serverPartitionKey;
        #endregion

        #region Public Properties
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
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
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public String PartitionFolder
        {
            get { return _partitionFolder; }
            set { _partitionFolder = value; }
        }
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        #endregion


        #region Static Methods
        static public ServerPartition Load(ServerEntityKey key)
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            ISelectServerPartition broker = read.GetBroker<ISelectServerPartition>();
            ServerPartition entity = broker.Load(key);
            read.Dispose();
            return entity;
        }
        #endregion


    }
}
