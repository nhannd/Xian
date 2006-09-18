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
        private int _serverid;
        private int _serverparentid;

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

        public int Serverid
        {
            get { return _serverid; }
            set { _serverid = value; }
        }

        public int Serverparentid
        {
            get { return _serverparentid; }
            set { _serverparentid = value; }
        }

        #endregion

        public AEServer(String servername, String serverpath, String serverlocation, String hostname, String aeTitle, int listenPort)
            : base(new HostName(hostname), new AETitle(aeTitle), new ListeningPort(listenPort))
        {
            _servername = servername;
            _serverpath = serverpath;
            _serverlocation = serverlocation;
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

        public override string ToString()
        {
            StringBuilder me = new StringBuilder();
            me.AppendFormat("{0} <{1}> ({2}) - {3}", Servername, Serverpath, Serverlocation, base.ToString());
            return me.ToString();
        }

        public string AEDetails
        {
            get
            {
                StringBuilder aeDescText = new StringBuilder();
                aeDescText.AppendFormat("AE Title: {0}\r\nHost: {1}\r\nListening Port: {2}\r\nLocation: {3}", base.AE, base.Host, base.Port, Serverlocation);
                return aeDescText.ToString();
            }
        }

        public String getServerGroupName()
        {
            if (_serverpath == null || _serverpath.Equals(""))
                return "";
            string[] gnames = getServerPathGroup();
            return gnames[gnames.Length-1]; 
        }

        public String[] getServerPathGroup()
        {
            if (_serverpath == null)
                return null;
            _serverpath = _serverpath.Replace("\\", "/").Trim();
            while (_serverpath.IndexOf("//") >= 0)
                _serverpath = _serverpath.Replace("//", "/");
            while (_serverpath.StartsWith("/"))
                _serverpath = _serverpath.Substring(1);
            while (_serverpath.EndsWith("/"))
                _serverpath = _serverpath.Substring(0, _serverpath.Length - 1);
            return _serverpath.Split('/');
        }

    }
}
