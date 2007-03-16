using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Shreds.ServerTree
{
    public interface IDicomServer
    {
        string ServerName { get; }
        string ServerPath { get; set; }
        bool IsServer { get; }
        string ServerDetails { get; }
    }

    public abstract class DicomServerBase : IDicomServer
    {
        private string _serverName;
        private string _serverPath;

        #region IDicomServer Members

        public abstract bool IsServer
        {
            get;
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }

        public abstract string ServerDetails
        {
            get;
        }

        #endregion
    }

}
