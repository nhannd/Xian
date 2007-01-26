using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [MenuAction("launch", "global-menus/Tools/DICOM Server")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class DICOMServerTestTool : Tool<IDesktopToolContext>
    {
        private DicomServerTestComponent _component;
        public void Launch()
        {
            if (_component == null)
            {
                _component = new DicomServerTestComponent();
                ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        _component,
                        "DICOM Server",
                        ShelfDisplayHint.DockRight,
                        delegate(IApplicationComponent component) { _component = null; });
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="DicomServerTestComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerTestComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerTestComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerTestComponentViewExtensionPoint))]
    public class DicomServerTestComponent : ApplicationComponent
    {
        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;
        private string _aeTitle;
        private string _saveDirectory;
        private int _port;
        private bool _isServerStarted;
        private int _progressBytes;
        private int _totalBytes;
        private string _sopInstanceUID;

        // Used by CMove to keep track of the sub-CStore progerss
        private SendParcel _sendParcel;
        private BackgroundTask _task;
        private int _lastSendParcelProgress = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerTestComponent()
        {
            DicomServerTree dicomServerTree = new DicomServerTree();
            if (dicomServerTree.CurrentServer != null)
            {
                DicomServer server = (DicomServer)dicomServerTree.CurrentServer;
                _aeTitle = server.DicomAE.AE;
                _port = server.DicomAE.Port;
            }
            else
            {
                _aeTitle = "STORESCP";
                _port = 4000;
            }

            _saveDirectory = ".\\dicom";
            _isServerStarted = false;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            StopServer();
            base.Stop();
        }

        public void StartServer()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle(_aeTitle), new ListeningPort(_port));

            CreateStorageDirectory(_saveDirectory);

            _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myOwnAEParameters, _saveDirectory);
            _dicomServer.FindScpEvent += OnFindScpEvent;
            _dicomServer.StoreScpBeginEvent += OnStoreScpBeginEvent;
            _dicomServer.StoreScpProgressingEvent += OnStoreScpProgressingEvent;
            _dicomServer.StoreScpEndEvent += OnStoreScpEndEvent;
            _dicomServer.MoveScpBeginEvent += OnMoveScpBeginEvent;
            _dicomServer.MoveScpProgressEvent += OnMoveScpProgressEvent;

            try
            {
                _dicomServer.Start();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            _isServerStarted = true;
        }

        public void StopServer()
        {
            if (_dicomServer != null)
            {
                _dicomServer.FindScpEvent -= OnFindScpEvent;
                _dicomServer.StoreScpBeginEvent -= OnStoreScpBeginEvent;
                _dicomServer.StoreScpProgressingEvent -= OnStoreScpProgressingEvent;
                _dicomServer.StoreScpEndEvent -= OnStoreScpEndEvent;
                _dicomServer.MoveScpBeginEvent -= OnMoveScpBeginEvent;
                _dicomServer.MoveScpProgressEvent -= OnMoveScpProgressEvent;

                _dicomServer.Stop();
                _dicomServer = null;
            }
            _isServerStarted = false;
        }

        #region Properties

        public string AETitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set { _saveDirectory = value; }
        }

        public bool IsServerStarted
        {
            get { return _isServerStarted; }
        }

        public string SOPInstanceUID
        {
            get { return _sopInstanceUID; }
        }

        public int ProgressBytes
        {
            get { return _progressBytes; }
        }

        public int TotalBytes
        {
            get { return _totalBytes; }
        }

        #endregion

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private void OnFindScpEvent(object sender, FindScpEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                e.QueryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(e.QueryKey);
            }
            catch (System.Data.SqlClient.SqlException exception)
            {
                ExceptionHandler.Report(exception, "MessageUnableToConnectToDataStore", this.Host.DesktopWindow);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
            }
        }

        private void OnStoreScpBeginEvent(object sender, StoreScpProgressUpdateEventArgs e)
        {
            // Receiving of new SOP Instance is about to beging, do something
            if (e == null)
                return;

            _sopInstanceUID = e.SOPInstanceUID;
            _progressBytes = (int) e.ProgressBytes;
            _totalBytes = (int) e.TotalBytes;

            NotifyPropertyChanged("SOPInstanceUID");
            NotifyPropertyChanged("ProgressBytes");
            NotifyPropertyChanged("TotalBytes");
        }

        private void OnStoreScpProgressingEvent(object sender, StoreScpProgressUpdateEventArgs e)
        {
            // More data for the new SOP Instance has arrived, update progress
            if (e == null)
                return;

            _progressBytes = (int) e.ProgressBytes;
            NotifyPropertyChanged("ProgressBytes");
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
            catch (System.Data.SqlClient.SqlException exception)
            {
                ExceptionHandler.Report(exception, "MessageUnableToConnectToDataStore", this.Host.DesktopWindow);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
            }
        }

        private void OnMoveScpBeginEvent(object sender, MoveScpEventArgs e)
        {
            if (e == null)
            {
                e.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                return;
            }

            if (_task == null || _task.IsRunning == false)
            {
                ApplicationEntity destinationAE = FindDestinationAE(e.MoveDestination);
                if (destinationAE == null)
                {
                    e.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_MoveDestinationUnknown;
                    return;
                }

                // Add it to send Queue
                ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle(_aeTitle), new ListeningPort(_port));
                _sendParcel = (SendParcel)DicomServicesLayer.GetISender(me).Send(new Uid(e.StudyInstanceUID), destinationAE, e.Description);
                _lastSendParcelProgress = 0;
                e.Response.NumberOfRemainingSubOperations = (ushort)_sendParcel.GetToSendObjectCount();

                // Start sending the parcel
                _task = new BackgroundTask(delegate(IBackgroundTaskContext context) { _sendParcel.StartSend(); }, false);
                _task.Run();
            }
        }

        private void OnMoveScpProgressEvent(object sender, MoveScpProgressEventArgs e)
        {
            if (e == null)
            {
                e.Response.DimseStatus = (ushort) OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                return;
            }

            // Keep the thread here and only reutrn CMoveRSP when there's an error or progress update
            while (_lastSendParcelProgress == _sendParcel.CurrentProgressStep)
            {
                switch (_sendParcel.GetState())
                {
                    case ParcelTransferState.Completed:
                        e.Response.NumberOfCompletedSubOperations++;
                        e.Response.NumberOfRemainingSubOperations = 0;
                        e.Response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                        break;
                    case ParcelTransferState.Cancelled:
                    case ParcelTransferState.CancelRequested:
                        e.Response.NumberOfWarningSubOperations++;
                        e.Response.NumberOfRemainingSubOperations--;
                        e.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Cancel_SubOperationsTerminatedDueToCancelIndication;
                        break;
                    case ParcelTransferState.Error:
                    case ParcelTransferState.Unknown:
                        e.Response.NumberOfFailedSubOperations++;
                        e.Response.NumberOfRemainingSubOperations--;
                        e.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                        break;
                    case ParcelTransferState.InProgress:
                    case ParcelTransferState.Paused:
                    case ParcelTransferState.PauseRequested:
                    case ParcelTransferState.Pending:
                    default:
                        e.Response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
                        System.Threading.Thread.Sleep(1000);
                        break;
                }

                if (e.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                {
                    _task = null;
                    _sendParcel = null;
                    return;
                }
            }

            e.Response.NumberOfCompletedSubOperations++;
            e.Response.NumberOfRemainingSubOperations--;
            _lastSendParcelProgress = _sendParcel.CurrentProgressStep;
        }

        private ApplicationEntity FindDestinationAE(string aeTitle)
        {
            // TODO: base on the AETitle passed in, find the server detail in the DicomServerTree
            // But the interface is difficult to use, so this is a stub until the DicomServerTree interface is improved

            DicomServerTree serverTree = new DicomServerTree();
            return FindDestinationAE(aeTitle, serverTree.MyServerGroup.ChildServers);
        }

        private ApplicationEntity FindDestinationAE(string aeTitle, List<IDicomServer> listServer)
        {
            if (listServer == null)
                return null;

            ApplicationEntity ae = null;
            foreach (IDicomServer ids in listServer)
            {
                if (ids.IsServer)
                {
                    DicomServer dicomServer = ids as DicomServer;
                    if (dicomServer != null && dicomServer.DicomAE.AE == aeTitle)
                        return dicomServer.DicomAE;
                }
                else
                {
                    DicomServerGroup serverGroup = ids as DicomServerGroup;
                    if (serverGroup != null)
                    {
                        ae = FindDestinationAE(aeTitle, serverGroup.ChildServers);
                        if (ae != null)
                            return ae;
                    }
                }
            }

            return null;
        }

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
    
    }
}
