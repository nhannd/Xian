#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Text;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public class ServerTreeDicomServer : IServerTreeDicomServer
    {
        public static readonly int DefaultPort = 104;
        public static readonly int DefaultHeaderServicePort = 50221;
        public static readonly int DefaultWadoServicePort = 1000;

        #region Private Fields

        private string _parentPath;
        private string _path;
        #endregion

        public ServerTreeDicomServer(string name)
        {
            Name = name;
            Port = DefaultPort;
            HeaderServicePort = DefaultHeaderServicePort;
            WadoServicePort = DefaultWadoServicePort;
        }

        internal ServerTreeDicomServer(IApplicationEntity entity)
        {
            Platform.CheckMemberIsSet(entity.ScpParameters, "ScpParameters");
            Name = entity.Name;
            Location = entity.Location;
            AETitle = entity.AETitle;
            HostName = entity.ScpParameters.HostName;
            Port = entity.ScpParameters.Port;
            IsStreaming = entity.StreamingParameters != null;
            if (IsStreaming)
            {
                HeaderServicePort = entity.StreamingParameters.HeaderServicePort;
                WadoServicePort = entity.StreamingParameters.WadoServicePort;
            }
            else
            {
                HeaderServicePort = DefaultHeaderServicePort;
                WadoServicePort = DefaultWadoServicePort;
            }
        }

        public ServerTreeDicomServer(
            string name,
            string location,
            string host,
            string aeTitle,
            int port,
            bool isStreaming,
            int headerServicePort,
            int wadoServicePort)
        {
            Name = name;
            Location = location;
            HostName = host;
            AETitle = aeTitle;
            Port = port;
            IsStreaming = isStreaming;
            HeaderServicePort = headerServicePort;
            WadoServicePort = wadoServicePort;
        }

        #region IServerTreeDicomServer Members

        public string AETitle { get; set; }

        public string Location { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public bool IsStreaming { get; set; }

        public int HeaderServicePort { get; set; }

        public int WadoServicePort { get; set; }

        #endregion

        #region IServerTreeNode Members

        public bool IsChecked { get; set; }

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

        public string Name { get; private set; }

        string IServerTreeNode.DisplayName
        {
            get { return Name; }
        }

        public string ParentPath
        {
            get { return _parentPath; }
        }

        #endregion

        public override string ToString()
        {
            var aeDescText = new StringBuilder();
            aeDescText.AppendFormat(SR.FormatServerDetails, Name, AETitle, HostName, Port);
            if (!string.IsNullOrEmpty(Location))
            {
                aeDescText.AppendLine();
                aeDescText.AppendFormat(SR.Location, Location);
            }
            if(IsStreaming)
            {
                aeDescText.AppendLine();
                aeDescText.AppendFormat(SR.FormatStreamingDetails, HeaderServicePort, WadoServicePort);
            }
            return aeDescText.ToString();
        }

        internal void ChangeParentPath(string newParentPath)
        {
            _parentPath = newParentPath ?? "";
            _path = _parentPath + Name;
        }
    }
}