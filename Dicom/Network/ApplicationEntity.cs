using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Describes the network parameters that define an Application Entity.
    /// </summary>
    public class ApplicationEntity
    {
        /// <summary>
        /// Gets the Hostname as a string.
        /// </summary>
        public string Host
        {
            get
            {
                return _hostname.ToString();
            }
        }

        /// <summary>
        /// Gets the AE Title as a string.
        /// </summary>
        public string AE
        {
            get
            {
                return _aeTitle.ToString();
            }
        }

        /// <summary>
        /// Gets the listening port as a string.
        /// </summary>
        public System.Int32 Port
        {
            get
            {
                return _listeningPort.ToInt32();
            }
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="hostname">The Hostname or IP address.</param>
        /// <param name="aeTitle">The AE Title.</param>
        /// <param name="listenPort">The listening port of the Application Entity.</param>
        public ApplicationEntity(HostName hostname, AETitle aeTitle, ListeningPort listenPort)
        {
            _hostname = hostname;
            _aeTitle = aeTitle;
            _listeningPort = listenPort;
        }

        private HostName _hostname;
        private AETitle _aeTitle;
        private ListeningPort _listeningPort;
    }
}
