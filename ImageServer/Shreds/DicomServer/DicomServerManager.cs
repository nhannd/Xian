using System;
using System.Collections.Generic;
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
                    DicomScpParameters parms = new DicomScpParameters(part, monitor, new FilesystemSelector(monitor));

                    DicomScp scp = new DicomScp(parms,AssociationVerifier.Verify);

                    scp.ListenPort = part.Port;
                    scp.AeTitle = part.AeTitle;

                    if (true == scp.Start())
                        _listenerList.Add(scp);
                    else
                    {
                        Platform.Log(LogLevel.Error, "Unable to add SCP handler for server partition {0}", part.Description);
                        Platform.Log(LogLevel.Error, "Partition {0} will not accept incoming DICOM associations.", part.Description);
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
