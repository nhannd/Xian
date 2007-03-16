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

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _serviceEndPointName;

        public DicomServerExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "DicomServerShred";
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked on Http port " + this.HttpPort.ToString() + " tcp port " + this.TcpPort.ToString());

			DicomServerManager.Instance.StartServer();

			StartHost<DicomMoveRequestServiceType, IDicomMoveRequestService>(_serviceEndPointName, "DicomServerShred");
        }

        public override void Stop()
        {
			DicomServerManager.Instance.StopServer();
			
			StopHost(_serviceEndPointName);

			Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
        }

        public override string GetDisplayName()
        {
            return "Dicom Server";
        }

        public override string GetDescription()
        {
            return "This shred hosts the Dicom Server and the WCF Dicom Move Request service";
        }
   }
}