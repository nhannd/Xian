using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
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
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked on Http port " + this.SharedHttpPort.ToString());

			DicomServerManager.Instance.StartServer();

			StartHttpHost<DicomServerServiceType, IDicomServerService>(_dicomServerEndpointName, "DicomServer");
        }

        public override void Stop()
        {
			StopHost(_dicomServerEndpointName);

			DicomServerManager.Instance.StopServer();
			
			Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
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