using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class PartitionSopClass : ServerEntity
    {
        #region Constructors
        public PartitionSopClass()
            : base("PartitionSopClass")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _sopClassKey;
        private String _sopClassUid;
        private String _description;
        private bool _nonImage;
        private bool _enabled;
        #endregion

        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerEntityKey SopClassKey
        {
            get { return _sopClassKey; }
            set { _sopClassKey = value; }
        }
        public String SopClassUid
        {
            get { return _sopClassUid; }
            set { _sopClassUid = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public bool NonImage
        {
            get { return _nonImage; }
            set { _nonImage = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

    }
}
