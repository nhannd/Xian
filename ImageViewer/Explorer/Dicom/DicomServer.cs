using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServer : DicomServerBase
    {
        public DicomServer(String servername, String serverpath, String serverlocation, String hostname, String aeTitle, int listenPort)
        {
            DicomAE = new ApplicationEntity(new HostName(hostname), new AETitle(aeTitle), new ListeningPort(listenPort));
            base.ServerName = servername;
            base.ServerPath = serverpath;
            ServerLocation = serverlocation;
        }

        public override bool IsServer
        {
            get
            {
                return true;
            }
        }

        public override string ServerDetails
        {
            get
            {
                StringBuilder aeDescText = new StringBuilder();
                aeDescText.AppendFormat("Server Name: {0}\r\nAE Title: {1}\r\nHost: {2}\r\nListening Port: {3}\r\nLocation: {4}", base.ServerName, _dicomAE.AE, _dicomAE.Host, _dicomAE.Port, ServerLocation);
                return aeDescText.ToString();
            }
        }

        #region DicomServer Members

        private ApplicationEntity _dicomAE;
        private string _serverLocation;

        public ApplicationEntity DicomAE
        {
            get { return _dicomAE; }
            set { _dicomAE = value; }
        }

        public string ServerLocation
        {
            get { return _serverLocation; }
            set { _serverLocation = value; }
        }

        #endregion

    }

}
