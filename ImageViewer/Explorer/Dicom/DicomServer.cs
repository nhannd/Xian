using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServer : DicomServerBase
    {
        public DicomServer(String serverName, String serverPath, String serverLocation, String hostName, String aeTitle, int listenPort)
        {
            DicomAE = new ApplicationEntity(new HostName(hostName), new AETitle(aeTitle), new ListeningPort(listenPort));
            base.ServerName = serverName;
            base.ServerPath = serverPath;
            ServerLocation = serverLocation;
        }

        public override bool IsServer
        {
            get { return true; }
        }

        public override string ServerDetails
        {
            get
            {
                StringBuilder aeDescText = new StringBuilder();
				aeDescText.AppendFormat(SR.FormatTooltipServerDetails, base.ServerName, _dicomAE.AE, _dicomAE.Host, _dicomAE.Port, ServerLocation);
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
