using System;
using System.Text;
using System.Xml.Serialization;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    public static class ServerExtensions
    {
        public static IDicomServerApplicationEntity ToApplicationEntity(this Server server)
        {
            if (server.IsStreaming)
                return new StreamingServerApplicationEntity(server.AETitle, server.Host, server.Port,
                                                            server.HeaderServicePort, server.WadoServicePort);

            return new DicomServerApplicationEntity(server.AETitle, server.Host, server.Port);
        }
    }

    [Serializable]
    public class Server : IServerTreeNode
    {
        #region Private Fields

        private bool _isChecked;
        private string _parentPath;
        private string _path;
        #endregion

        protected Server()
        {
        }

        public Server(
            string name, 
            string location, 
            string host, 
            string aeTitle, 
            int port, 
            bool isStreaming,
            int headerServicePort,
            int wadoServicePort)
        {
            NameOfServer = name;
            Location = location;
            Host = host;
            AETitle = aeTitle;
            Port = port;
            IsStreaming = isStreaming;
            HeaderServicePort = headerServicePort;
            WadoServicePort = wadoServicePort;
        }

        #region Public Fields

        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String NameOfServer { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String Location { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String Host { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String AETitle { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public bool IsStreaming { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int HeaderServicePort { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int WadoServicePort { get; set; }

        #endregion

        #region IServerTreeNode Members

        [XmlIgnore]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public bool IsLocalDataStore
        {
            get { return false; }
        }

        public bool IsServer
        {
            get { return true; }
        }

        public bool IsServerGroup
        {
            get { return false; }
        }

        public string Path
        {
            get { return _path; }
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public string Name
        {
            get { return NameOfServer; }
        }

        string IServerTreeNode.DisplayName
        {
            get { return Name; }
        }

        public string ParentPath
        {
            get { return _parentPath; }
        }

        #endregion

        internal void ChangeParentPath(string newParentPath)
        {
            _parentPath = newParentPath ?? "";
            _path = _parentPath + this.Name;
        }

        public override string ToString()
        {
            var aeDescText = new StringBuilder();
            aeDescText.AppendFormat(SR.FormatServerDetails, this.Name, this.AETitle, this.Host, this.Port);
            if (!string.IsNullOrEmpty(this.Location))
            {
                aeDescText.AppendLine();
                aeDescText.AppendFormat(SR.Location, this.Location);
            }
            if(IsStreaming)
            {
                aeDescText.AppendLine();
                aeDescText.AppendFormat(SR.FormatStreamingDetails, this.HeaderServicePort, this.WadoServicePort);
            }
            return aeDescText.ToString();
        }
    }
}