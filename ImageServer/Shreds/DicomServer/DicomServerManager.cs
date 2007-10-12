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

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.ImageServer;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Parameters;


namespace ClearCanvas.ImageServer.Shreds.DicomServer
{
    /// <summary>
    /// This class manages the DICOM SCP Shred for the ImageServer.
    /// </summary>
    public class DicomServerManager
    {
        #region Private Members
        private List<DicomScp> _listenerList = new List<DicomScp>();
        private static DicomServerManager _instance;
        #endregion

        #region Contructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public DicomServerManager()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Singleton instance of the class.
        /// </summary>
        public static DicomServerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DicomServerManager();

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion

        
        #region Public Methods
        /// <summary>
        /// Method called when starting the DICOM SCP.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method starts a <see cref="DicomScp"/> instance for each server partition configured in
        /// the database.  It assumes that the combination of the configured AE Title and Port for the 
        /// partition is unique.  
        /// </para>
        /// </remarks>
        public void Start()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            IReadContext read = store.OpenReadContext();

            IGetServerPartitions broker = read.GetBroker<IGetServerPartitions>();
            IList<ServerPartition> partitions = broker.Execute();
            FilesystemMonitor monitor = new FilesystemMonitor();
            
            monitor.Load();

            read.Dispose();
            
            foreach (ServerPartition part in partitions)
            {
                if (part.Enabled)
                {
                    DicomScpParameters parms =
                            new DicomScpParameters(part, monitor, new FilesystemSelector(monitor));

                    if (ImageServerShredSettings.Default.ListenIPV4)
                    {
                        DicomScp ipV4Scp = new DicomScp(parms, AssociationVerifier.Verify);

                        ipV4Scp.ListenPort = part.Port;
                        ipV4Scp.AeTitle = part.AeTitle;

                        if (ipV4Scp.Start(IPAddress.Any))
                            _listenerList.Add(ipV4Scp);
                        else
                        {
                            Platform.Log(LogLevel.Error, "Unable to add IPv4 SCP handler for server partition {0}",
                                         part.Description);
                            Platform.Log(LogLevel.Error,
                                         "Partition {0} will not accept IPv4 incoming DICOM associations.",
                                         part.Description);
                        }
                    }

                    if (ImageServerShredSettings.Default.ListenIPV6)
                    {
                        DicomScp ipV6Scp = new DicomScp(parms, AssociationVerifier.Verify);

                        ipV6Scp.ListenPort = part.Port;
                        ipV6Scp.AeTitle = part.AeTitle;

                        if (ipV6Scp.Start(IPAddress.IPv6Any))
                            _listenerList.Add(ipV6Scp);
                        else
                        {
                            Platform.Log(LogLevel.Error, "Unable to add IPv6 SCP handler for server partition {0}",
                                         part.Description);
                            Platform.Log(LogLevel.Error,
                                         "Partition {0} will not accept IPv6 incoming DICOM associations.",
                                         part.Description);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method called when stopping the DICOM SCP.
        /// </summary>
        public void Stop()
        {
            foreach (DicomScp scp in _listenerList)
            {
                scp.Stop();
            }

        }
        #endregion
    }
}
