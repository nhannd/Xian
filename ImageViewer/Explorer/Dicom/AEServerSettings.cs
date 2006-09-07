using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [Serializable]
    public class AEServerSettings
    {
        public AEServerSettings()
        {
        }

        public AEServerSettings(String servername, String serverpath, String description, String hostname, String aeTitle, int listenPort)
        {
            _servername = servername;
            _serverpath = serverpath;
            _description = description;
            _hostname = hostname;
            _aeTitle = aeTitle;
            _listenPort = listenPort;
        }

        public override string ToString()
        {
            StringBuilder me = new StringBuilder();
            me.AppendFormat("{0} - {1} ({2}) <{3} / {4}:{5}>",
                _servername, _serverpath, _description, _aeTitle, _hostname, _listenPort.ToString());
            return me.ToString();
        }

        public String Servername
        {
            get { return _servername; }
        }

        public String Description
        {
            get { return _description; }
        }

        public String Hostname
        {
            get { return _hostname; }
            //set { _hostname = value; }
        }

        public int ListenPort
        {
            get { return _listenPort; }
            //set { _listenPort = value; }
        }

        public String AeTitle
        {
            get { return _aeTitle; }
            //set { _aeTitle = value; }
        }


        public String Serverpath
        {
            get { return _serverpath; }
            //set { _serverpath = value; }
        }

        public String _servername;
        public String _description;
        public String _serverpath;
        public String _hostname;
        public String _aeTitle;
        public int _listenPort;

    }
}
