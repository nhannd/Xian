using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;

using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DiskspaceManagerExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _diskspaceManagerEndpointName;

        public DiskspaceManagerExtension()
        {
            _className = this.GetType().ToString();
            _diskspaceManagerEndpointName = "DiskspaceManager";
        }

        public override void Start()
        {
			Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");

			DiskspaceManagerProcessor.Instance.StartProcessor();

			StartNetPipeHost<DiskspaceManagerServiceType, IDiskspaceManagerService>(_diskspaceManagerEndpointName, "DiskspaceManager");
        }

        public override void Stop()
        {
			StopHost(_diskspaceManagerEndpointName);
			
			DiskspaceManagerProcessor.Instance.StopProcessor();
			
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
        }

        public override string GetDisplayName()
        {
			return SR.DiskspaceManager;
        }

        public override string GetDescription()
        {
			return SR.DiskspaceManagerDescription;
        }
   }
}