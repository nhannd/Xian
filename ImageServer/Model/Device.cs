using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model
{
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
        #endregion
    }
}
