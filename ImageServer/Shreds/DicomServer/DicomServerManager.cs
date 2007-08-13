using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.ImageServer;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;


namespace ClearCanvas.ImageServer.Shreds.DicomServer
{
    public class DicomServerManager
    {
        #region Private Members
        private List<DicomScp> _listenerList = new List<DicomScp>();
        private static DicomServerManager _instance;
        #endregion

        #region Contructors
        public DicomServerManager()
        {
        }
        #endregion

        #region Properties
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
        public void Start()
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            IReadContext read = store.OpenReadContext();

            IGetServerPartitions broker = read.GetBroker<IGetServerPartitions>();
            IList<ServerPartition> partitions = broker.Execute();
            
            read.Dispose();
            
            foreach (ServerPartition part in partitions)
            {
                if (part.Enabled)
                {
                    DicomScpParameters parms = new DicomScpParameters(part);

                    DicomScp scp = new DicomScp(parms);

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
