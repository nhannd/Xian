using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class DicomServerBase : IDicomServer
    {
        #region DicomServerBase Members

        private string _serverName;
        private string _serverPath;

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
