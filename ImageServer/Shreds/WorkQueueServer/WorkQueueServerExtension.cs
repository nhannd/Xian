using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Server.ShredHost;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Shreds.WorkQueueServer
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class WorkQueueServerExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _workQueueServerEndpointName;

        public WorkQueueServerExtension()
        {
            _className = this.GetType().ToString();
            _workQueueServerEndpointName = "WorkQueueServer";
        }

        public override void Start()
        {
            Platform.Log(LogLevel.Info,"{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

            
            //DicomServerManager.Instance.Start();

            //StartNetPipeHost<DicomServerServiceType, IDicomServerService>(_workQueueServerEndpointName, "DicomServer");
        }

        public override void Stop()
        {
            StopHost(_workQueueServerEndpointName);

            //DicomServerManager.Instance.Stop();

            Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
        }

        public override string GetDisplayName()
        {
            return SR.WorkQueueServer;
        }

        public override string GetDescription()
        {
            return SR.WorkQueueServerDescription;
        }
    }
}
