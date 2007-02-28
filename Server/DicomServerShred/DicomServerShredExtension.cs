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
using ClearCanvas.Dicom.Services;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.DicomServerShred
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerShredExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _serviceEndPointName;
        private EventWaitHandle _stopSignal;

        private DicomServerEventManager _serverManager;

        public DicomServerShredExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "DicomServerShred";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();

            StartHost<DicomServerShredServiceType, IDicomServerShredInterface>(_serviceEndPointName, "DicomServerShred");

            // start up processing thread
            Thread t = new Thread(new ThreadStart(DefaultShredRoutine));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();

            // wait for processing thread to finish
            t.Join();

            // stop hosting the service
            StopHost(_serviceEndPointName);
        }

        public override void Stop()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            _stopSignal.Set();
        }

        public override string GetDisplayName()
        {
            return "DicomServerShredExtension";
        }

        public override string GetDescription()
        {
            return "This shred starts a DICOM Server";
        }

        private void DefaultShredRoutine()
        {
            StartServer();

            // Give server some time to start and update its state
            System.Threading.Thread.Sleep(100);

            while (_serverManager != null && _serverManager.IsServerRunning)
            {
                System.Threading.Thread.Sleep(1000);

                // wait for host's stop signal
                if (_stopSignal.WaitOne(100, false))
                    break;
            }

            StopServer();
        }

        public void StartServer()
        {
            try
            {
                //WCF TODO: Should load the settings from a WCF service
                DicomServerTree serverTree = new DicomServerTree();
                DicomServer server = (DicomServer)serverTree.CurrentServer;

                _serverManager = new DicomServerEventManager(server.DicomAE.Host, server.DicomAE.AE, server.DicomAE.Port, server.ServerPath + "\\dicom");

                _serverManager.StartServer();
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Error);
            }
        }

        public void StopServer()
        {
            try
            {
                if (_serverManager != null && _serverManager.IsServerRunning)
                    _serverManager.StopServer();
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Error);
            }

            _serverManager = null;
        }

   }
}