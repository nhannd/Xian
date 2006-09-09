namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Describes the network parameters that define an Application Entity.
    /// </summary>
    [SerializableAttribute]
    public class ApplicationEntity
    {
        /// <summary>
        /// Gets the Hostname as a string.
        /// </summary>
        public String Host
        {
            get { return _hostname.ToString(); }
            private set { _hostname = new HostName(value); }
        }

        /// <summary>
        /// Gets the AE Title as a string.
        /// </summary>
        public String AE
        {
            get { return _aeTitle.ToString(); }
            private set { _aeTitle = new AETitle(value); }
        }

        /// <summary>
        /// Gets the listening port as a string.
        /// </summary>
        public int Port
        {
            get { return _listeningPort.ToInt(); }
            private set { _listeningPort = new ListeningPort(value); }
        }
        
        /// <summary>
        /// Sets the timeout value for making DICOM associations
        /// and network connections to the target/remote Application
        /// Entity.
        /// </summary>
        public ushort ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        /// <summary>
        /// Sets the timeout value for completing network operations
        /// such as a single C-STORE command.
        /// </summary>
        public ushort OperationTimeout
        {
            get { return _operationTimeout; }
            set { _operationTimeout = value; }
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="hostname">The Hostname or IP address.</param>
        /// <param name="aeTitle">The AE Title.</param>
        /// <param name="listenPort">The port on which this target AE will listen for incoming associations.</param>
        public ApplicationEntity(HostName hostname, AETitle aeTitle, ListeningPort listenPort)
        {
            _hostname = hostname;
            _aeTitle = aeTitle;
            _listeningPort = listenPort;
        }

        /// <summary>
        /// Overloaded version of the constructor that allows client to
        /// set the connection timeout value.
        /// </summary>
        /// <param name="hostname">The Hostname or IP address.</param>
        /// <param name="aeTitle">The AE Title.</param>
        /// <param name="listenPort">The port on which this AE will listen for incoming associations.</param>
        /// <param name="connectionTimeout">The timeout value for making associations and network connections.</param>
        public ApplicationEntity(HostName hostname, AETitle aeTitle, ListeningPort listenPort, ushort connectionTimeout)
            : this(hostname, aeTitle, listenPort)
        {
            _connectionTimeout = connectionTimeout;
        }

        /// <summary>
        /// Overloaded version of the constructor that allows client to set both the
        /// connection timeout and operation timeout.
        /// </summary>
        /// <param name="hostname">The Hostname or IP address.</param>
        /// <param name="aeTitle">The AE Title.</param>
        /// <param name="listenPort">The port on which this AE will listen for incoming associations.</param>
        /// <param name="connectionTimeout">The timeout value for making associations and network connections.</param>
        /// <param name="operationTimeout">The timeout value for individual DICOM commands, such as C-STORE.</param>
        public ApplicationEntity(HostName hostname, AETitle aeTitle, ListeningPort listenPort, ushort connectionTimeout, ushort operationTimeout)
            : this(hostname, aeTitle, listenPort, connectionTimeout)
        {
            _operationTimeout = operationTimeout;
        }

        /// <summary>
        /// Mandatory parameterless constructor for Hibernate
        /// </summary>
        private ApplicationEntity()
        {
        }

        /// <summary>
        /// Provides a string representation of an Application Entity.
        /// </summary>
        /// <returns>The String representation.</returns>D
        public override string ToString()
        {
            StringBuilder me = new StringBuilder();
            me.AppendFormat("{0} / {1}:{2}", AE, Host, Port);
            return me.ToString();
        }

        private HostName _hostname;
        private AETitle _aeTitle;
        private ListeningPort _listeningPort;
        private ushort _connectionTimeout = 20;
        private ushort _operationTimeout = 180;
    }
}
