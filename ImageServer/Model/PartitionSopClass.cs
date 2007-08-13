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
        private ServerEntityKey _serverPartitionRef;
        private ServerEntityKey _sopClassRef;
        private String _sopClassUid;
        private String _description;
        private bool _nonImage;
        private bool _enabled;
        #endregion

        public ServerEntityKey ServerPartitionRef
        {
            get { return _serverPartitionRef; }
            set { _serverPartitionRef = value; }
        }
        public ServerEntityKey SopClassRef
        {
            get { return _sopClassRef; }
            set { _sopClassRef = value; }
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
