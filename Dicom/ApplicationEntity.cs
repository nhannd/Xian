using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public struct ApplicationEntity
    {
        private HostName _hostname;
        private AETitle _aeTitle;
        private ListeningPort _listeningPort;

        public string Host
        {
            get
            {
                return _hostname.ToString();
            }
        }

        public string AE
        {
            get
            {
                return _aeTitle.ToString();
            }
        }

        public System.Int32 Port
        {
            get
            {
                return _listeningPort.ToInt32();
            }
        }

        public ApplicationEntity(HostName hostname, AETitle aeTitle, ListeningPort listenPort)
        {
            _hostname = hostname;
            _aeTitle = aeTitle;
            _listeningPort = listenPort;
        }
    }
}
