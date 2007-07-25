using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class ServerPartition : ProcedureEntity
    {
        #region Private Members
        private bool _enabled;
        private String _description;
        private string _aeTitle;
        private int _port;
        private string _folderName;
        #endregion

        #region Public Properties
        public bool Enabled
        {
            get { return _enabled; }
        }

        public String Description
        {
            get { return _description; }
        }

        public String AeTitle
        {
            get { return _aeTitle; }
        }

        public int Port
        {
            get { return _port; }
        }

        public String FolderName
        {
            get { return _folderName; }
        }
        #endregion
    }
}
