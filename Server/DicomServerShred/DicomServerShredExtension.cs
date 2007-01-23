using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;

using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Server.DicomServerShred
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerShredExtension : ShredBase
    {
        private readonly string _className;
        private EventWaitHandle _stopSignal;

        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;
        private string _saveDirectory;
        private string _aeTitle;
        private int _port;

        public DicomServerShredExtension()
        {
            _className = this.GetType().ToString();
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");

            //TODO: Should load the settings from a configuration
            _aeTitle = "AETITLE";
            _port = 4006;
            _saveDirectory = ".\\dicom";
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();

            // start up processing thread
            Thread t = new Thread(new ThreadStart(DefaultShredRoutine));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();

            // wait for processing thread to finish
            t.Join();
        }

        public override void Stop()
        {
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

            while (_dicomServer.IsRunning)
            {
                System.Threading.Thread.Sleep(1000);

                // wait for host's stop signal
                if (_stopSignal.WaitOne(100, false))
                    break;
            }

            StopServer();        
        }

        private void StartServer()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle(_aeTitle), new ListeningPort(_port));

            CreateStorageDirectory(_saveDirectory);
            _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myOwnAEParameters, _saveDirectory);

            try
            {
                _dicomServer.Start();

                // Register for event 
                _dicomServer.FindScpEvent += OnFindScpEvent;
                _dicomServer.StoreScpBeginEvent += OnStoreScpBeginEvent;
                _dicomServer.StoreScpProgressingEvent += OnStoreScpProgressingEvent;
                _dicomServer.StoreScpEndEvent += OnStoreScpEndEvent;
            }
            catch (Exception e)
            {
                _dicomServer = null;
                Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: throws exception @ StartServer" + e.Message);
            }
        }

        private void StopServer()
        {
            if (_dicomServer != null && _dicomServer.IsRunning)
            {
                _dicomServer.FindScpEvent -= OnFindScpEvent;
                _dicomServer.StoreScpBeginEvent -= OnStoreScpBeginEvent;
                _dicomServer.StoreScpProgressingEvent -= OnStoreScpProgressingEvent;
                _dicomServer.StoreScpEndEvent -= OnStoreScpEndEvent;
                _dicomServer.Stop();
                _dicomServer = null;
            }
        }

        #region Event Handler

        private void OnFindScpEvent(object sender, FindScpEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                e.QueryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(e.QueryKey);
            }
            catch (Exception exception)
            {
                Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: throws exception at FindScpEvent " + exception.Message);
            }
        }

        private void OnStoreScpBeginEvent(object sender, StoreScpProgressUpdateEventArgs e)
        {
            // Receiving of new SOP Instance is about to beging, do something
            if (e == null)
                return;
        }

        private void OnStoreScpProgressingEvent(object sender, StoreScpProgressUpdateEventArgs e)
        {
            // More data for the new SOP Instance has arrived, update progress
            if (e == null)
                return;
        }

        private void OnStoreScpEndEvent(object sender, StoreScpImageReceivedEventArgs e)
        {
            // A new SOP Instance has been written successfully to the disk, update database
            if (e == null)
                return;

            try
            {
                InsertSopInstance(e.FileName);
            }
            catch (Exception exception)
            {
                Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: throws exception at StoreScpEndEvent " + exception.Message);
            }
        }

        #endregion

        #region Private Helper functions

        private void CreateStorageDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Indirectly hooked up to DicomServer.OnStoreScpEndEvent to 
        /// store the instance object when it arrives.
        /// </summary>
        /// <param name="fileName">Path to the file</param>
        private void InsertSopInstance(string fileName)
        {
            DcmFileFormat file = new DcmFileFormat();
            OFCondition condition = file.loadFile(fileName);
            if (!condition.good())
            {
                // there was an error reading the file, possibly it's not a DICOM file
                return;
            }

            DcmMetaInfo metaInfo = file.getMetaInfo();
            DcmDataset dataset = file.getDataset();

            if (ConfirmProcessableFile(metaInfo, dataset))
            {
                IDicomPersistentStore dicomStore = DataAccessLayer.GetIDicomPersistentStore();
                dicomStore.InsertSopInstance(metaInfo, dataset, fileName);
                dicomStore.Flush();
            }

            // keep the file object alive until the end of this scope block
            // otherwise, it'll be GC'd and metaInfo and dataset will be gone
            // as well, even though they are needed in the InsertSopInstance
            // and sub methods
            GC.KeepAlive(file);
        }

        /// <summary>
        /// Determine various characteristics to see whether we can actually
        /// store this file. For retrievals this should never be a problem. For
        /// DatabaseRebuild, sometimes objects are stored without their Group 2
        /// tags, which makes them impossible to process, i.e. we'd have to guess
        /// correctly the transfer syntax.
        /// </summary>
        /// <param name="metaInfo">Group 2 (metaheader) tags</param>
        /// <param name="dataset">DICOM header</param>
        /// <returns></returns>
        private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
        {
            StringBuilder stringValue = new StringBuilder(1024);
            OFCondition cond;
            cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
            if (cond.good())
            {
                // we want to skip Media Storage Directory Storage (DICOMDIR directories)
                if ("1.2.840.10008.1.3.10" == stringValue.ToString())
                    return false;
            }

            return true;
        }

        #endregion

    }
}