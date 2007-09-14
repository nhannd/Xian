using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model
{
    public class Filesystem : ServerEntity
    {
        #region Constructors
        public Filesystem()
            : base("Filesystem")
        {
        }
        #endregion

        #region Private Members
        private String _path;
        private bool _enabled;
        private bool _readOnly;
        private bool _writeOnly;
        private String _description;
        private ServerEntityKey _fileSystemTierKey;
        #endregion

        #region Public Properties
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }
        public bool WriteOnly
        {
            get { return _writeOnly; }
            set { _writeOnly = value; }
        }
        public String FilesystemPath
        {
            get { return _path; }
            set { _path = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ServerEntityKey FilesystemTierKey
        {
            get { return _fileSystemTierKey; }
            set { _fileSystemTierKey = value; }
        }
        #endregion

    }
}
