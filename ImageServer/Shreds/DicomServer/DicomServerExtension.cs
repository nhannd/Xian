using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Shreds.DicomServer
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _dicomServerEndpointName;

        public DicomServerExtension()
        {
            _className = this.GetType().ToString();
            _dicomServerEndpointName = "DicomServer";
        }

        public override void Start()
        {
            Platform.Log(LogLevel.Info,"{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

            IReadContext read = store.OpenReadContext();

            IGetServerPartitions broker = read.GetBroker<IGetServerPartitions>();
            IList<ServerPartition> partitions = broker.Execute();

            //DicomServerManager.Instance.Start();

            //StartNetPipeHost<DicomServerServiceType, IDicomServerService>(_workQueueServerEndpointName, "DicomServer");
        }

        public override void Stop()
        {
            StopHost(_dicomServerEndpointName);

            //DicomServerManager.Instance.Stop();

            Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
        }

        public override string GetDisplayName()
        {
            return SR.DicomServer;
        }

        public override string GetDescription()
        {
            return SR.DicomServerDescription;
        }
    }
}
