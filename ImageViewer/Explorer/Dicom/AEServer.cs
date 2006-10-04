using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class AEServer : ApplicationEntity
    {
        #region Fields

        private String _servername;
        private String _serverpath;
        private String _serverlocation;
        private String[] _serverPathGroups;
        private int _serverID;
        private int _serverParentID;

        public String Servername
        {
            get { return _servername; }
            set { _servername = value; }
        }

        public String Serverpath
        {
            get { return _serverpath; }
            set { _serverpath = value; }
        }

        public String Serverlocation
        {
            get { return _serverlocation; }
            set { _serverlocation = value; }
        }

        public String[] ServerPathGroups
        {
            get {
                if (_serverPathGroups == null)
                    BuildServerPathGroup();
                return _serverPathGroups; 
            }
            set { _serverPathGroups = value; }
        }

        public int ServerID
        {
            get { return _serverID; }
            set { _serverID = value; }
        }

        public int ServerParentID
        {
            get { return _serverParentID; }
            set { _serverParentID = value; }
        }

        #endregion

        public AEServer(String servername, String serverpath, String serverlocation, String hostname, String aeTitle, int listenPort)
            : base(new HostName(hostname), new AETitle(aeTitle), new ListeningPort(listenPort))
        {
            _servername = servername;
            _serverpath = serverpath;
            _serverlocation = serverlocation;
            _serverPathGroups = null;
            _serverID = -1;
            _serverParentID = -1;
        }

        public new String Host
        {
            get { return base.Host; }
        }

        public new String AE
        {
            get { return base.AE; }
        }

        public new int Port
        {
            get { return base.Port; }
        }

/*        public override string ToString()
        {
            StringBuilder me = new StringBuilder();
            me.AppendFormat("{0} <{1}> ({2}) - {3}", Servername, Serverpath, Serverlocation, base.ToString());
            return me.ToString();
        }
*/
        public string AEDetails
        {
            get
            {
                StringBuilder aeDescText = new StringBuilder();
                aeDescText.AppendFormat("Server Name: {0}\r\nAE Title: {1}\r\nHost: {2}\r\nListening Port: {3}\r\nLocation: {4}", Servername, base.AE, base.Host, base.Port, Serverlocation);
                return aeDescText.ToString();
            }
        }

        public ApplicationEntity getApplicationEntity()
        {
            return(new ApplicationEntity(new HostName(base.Host), new AETitle(base.AE), new ListeningPort(base.Port)));
        }

        public String getServerGroupName()
        {
            if (ServerPathGroups == null || ServerPathGroups.Length <= 0)
                return "";
            return ServerPathGroups[ServerPathGroups.Length - 1];
        }

        public String getServerGroupID()
        {
            if (Servername == null || ServerPathGroups == null || ServerPathGroups.Length <= 0)
                return "";
            if (Servername.Equals(AENavigatorComponent.EmptyNodeName))
            {
                if (ServerPathGroups.Length == 1)
                    return Serverpath;

                string spath = "";
                for (int m = 0; m < ServerPathGroups.Length-1; m++)
                {
                    spath += "/" + ServerPathGroups[m];
                }
                return spath;
            }
            else
            {
                return Serverpath;
            }
        }

        public String getServerRootName()
        {
            if (ServerPathGroups == null || ServerPathGroups.Length <= 0)
                return "";
            return ServerPathGroups[0];
        }

        private int BuildServerPathGroup()
        {
            if (_serverpath == null)
                return -1;
            _serverpath = _serverpath.Replace("\\", "/").Trim();
            while (_serverpath.IndexOf("//") >= 0)
                _serverpath = _serverpath.Replace("//", "/");
            while (_serverpath.StartsWith("/"))
                _serverpath = _serverpath.Substring(1);
            while (_serverpath.EndsWith("/"))
                _serverpath = _serverpath.Substring(0, _serverpath.Length - 1);
            _serverPathGroups = _serverpath.Split('/');
            return _serverPathGroups.Length;
        }
    }
}
