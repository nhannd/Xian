#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Dicom.OffisNetwork
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
        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        /// <summary>
        /// Sets the timeout value for completing network operations
        /// such as a single C-STORE command.
        /// </summary>
        public int OperationTimeout
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
        private int _connectionTimeout = 20;
        private int _operationTimeout = 180;
    }
}
