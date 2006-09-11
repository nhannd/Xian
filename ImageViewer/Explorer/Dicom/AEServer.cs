using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class AEServer : ApplicationEntity
    {
        public AEServer(String servername, String serverpath, String description, String hostname, String aeTitle, int listenPort)
            : base(new HostName(hostname), new AETitle(aeTitle), new ListeningPort(listenPort))
        {
            _servername = servername;
            _serverpath = serverpath;
            _description = description;
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
            me.AppendFormat("{0} <{1}> ({2}) - {3}", Servername, Serverpath, Description, base.ToString());
            return me.ToString();
        }

        private String _servername;
        private String _description;
        private String _serverpath;

        public String Serverpath
        {
            get { return _serverpath; }
            set { _serverpath = value; }
        }

        public String Servername
        {
            get { return _servername; }
            set { _servername = value; }
        }

        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
