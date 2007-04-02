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
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked on Http port " + this.SharedHttpPort.ToString());

            StartHttpHost<DiskspaceManagerServiceType, IDiskspaceManagerService>(_diskspaceManagerEndpointName, "DiskspaceManager");

            DiskspaceManagerProcessor.Instance.StartProcessor();

        }

        public override void Stop()
        {
            DiskspaceManagerProcessor.Instance.StopProcessor();

            StopHost(_diskspaceManagerEndpointName);

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