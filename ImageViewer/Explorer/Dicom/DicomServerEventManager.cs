using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServerEventManager
    {
        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;

        // Used by CFind
        private ReadOnlyQueryResultCollection _queryResults;
        private int _resultIndex;

        // Used by CMove to keep track of the sub-CStore progerss
        private SendParcel _sendParcel;
        private BackgroundTask _task;
        private int _lastSendParcelProgress = 0;

        public DicomServerEventManager()
        {
        }

        public void StartServer()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle(LocalApplicationEntity.AETitle), new ListeningPort(LocalApplicationEntity.Port));

            CreateStorageDirectory(LocalApplicationEntity.DicomStoragePath);

            _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myOwnAEParameters, LocalApplicationEntity.DicomStoragePath);
            _dicomServer.FindScpEvent += OnFindScpEvent;
            _dicomServer.FindScpProgressEvent += OnFindScpProgressEvent;
            _dicomServer.StoreScpBeginEvent += OnStoreScpBeginEvent;
            _dicomServer.StoreScpProgressEvent += OnStoreScpProgressEvent;
            _dicomServer.StoreScpEndEvent += OnStoreScpEndEvent;
            _dicomServer.MoveScpBeginEvent += OnMoveScpBeginEvent;
            _dicomServer.MoveScpProgressEvent += OnMoveScpProgressEvent;

            _dicomServer.Start();
        }

        public void StopServer()
        {
            if (_dicomServer != null)
            {
                _dicomServer.FindScpEvent -= OnFindScpEvent;
                _dicomServer.FindScpProgressEvent -= OnFindScpProgressEvent;
                _dicomServer.StoreScpBeginEvent -= OnStoreScpBeginEvent;
                _dicomServer.StoreScpProgressEvent -= OnStoreScpProgressEvent;
                _dicomServer.StoreScpEndEvent -= OnStoreScpEndEvent;
                _dicomServer.MoveScpBeginEvent -= OnMoveScpBeginEvent;
                _dicomServer.MoveScpProgressEvent -= OnMoveScpProgressEvent;

                _dicomServer.Stop();
                _dicomServer = null;
            }
        }

        #region Properties

        public string AETitle
        {
            get { return LocalApplicationEntity.AETitle; }
        }

        public int Port
        {
            get { return LocalApplicationEntity.Port; }
        }

        public string SaveDirectory
        {
            get { return LocalApplicationEntity.DicomStoragePath; }
        }

        public bool IsServerRunning
        {
            get { return (_dicomServer == null ? false : _dicomServer.IsRunning); }
        }

        #endregion

        #region DicomServer Event Handlers

        private void OnFindScpEvent(object sender, DicomServerEventArgs e)
        {
            InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            info.Response.DimseStatus = (ushort) QueryDB(info.RequestIdentifiers);
        }

        private void OnFindScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            info.Response.DimseStatus = (ushort)GetNextQueryResult(info.ResponseIdentifiers);
        }

        private void OnStoreScpBeginEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            // TODO: These event may have to be marshalled
            //SopInstanceReceivedEventArgs arg = new SopInstanceReceivedEventArgs(...)
            //EventsHelper.Fire(_sopInstanceReceivedEvent, this, arg);
        }

        private void OnStoreScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            //SopInstanceReceivedEventArgs arg = new SopInstanceReceivedEventArgs(...)
            //EventsHelper.Fire(_sopInstanceReceivedEvent, this, arg);
        }

        private void OnStoreScpEndEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            // A new SOP Instance has been written successfully to the disk, update database
            info.DimseStatus = (ushort) InsertSopInstance(info.FileName);

            //SopInstanceReceivedEventArgs arg = new SopInstanceReceivedEventArgs(...)
            //EventsHelper.Fire(_sopInstanceReceivedEvent, this, arg);
        }

        private void OnMoveScpBeginEvent(object sender, DicomServerEventArgs e)
        {
            InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            // Start the Query
            info.Response.DimseStatus = (ushort)QueryDB(info.RequestIdentifiers);
            if (info.Response.DimseStatus != (ushort) OffisDcm.STATUS_Pending)
                return;

            // Verify we have return results
            info.Response.DimseStatus = (ushort)GetNextQueryResult(info.ResponseIdentifiers);
            if (info.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                return;

            info.Response.DimseStatus = (ushort)StartMove(info);
        }

        private void OnMoveScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            // Keep the thread here and only reutrn CMoveRSP when there's an error or progress update
            while (_lastSendParcelProgress == _sendParcel.CurrentProgressStep)
            {
                switch (_sendParcel.GetState())
                {
                    case ParcelTransferState.Completed:
                        info.Response.NumberOfCompletedSubOperations++;
                        info.Response.NumberOfRemainingSubOperations = 0;
                        info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                        break;
                    case ParcelTransferState.Cancelled:
                    case ParcelTransferState.CancelRequested:
                        info.Response.NumberOfWarningSubOperations++;
                        info.Response.NumberOfRemainingSubOperations--;
                        info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Cancel_SubOperationsTerminatedDueToCancelIndication;
                        break;
                    case ParcelTransferState.Error:
                    case ParcelTransferState.Unknown:
                        info.Response.NumberOfFailedSubOperations++;
                        info.Response.NumberOfRemainingSubOperations--;
                        info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                        break;
                    case ParcelTransferState.InProgress:
                    case ParcelTransferState.Paused:
                    case ParcelTransferState.PauseRequested:
                    case ParcelTransferState.Pending:
                    default:
                        info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
                        System.Threading.Thread.Sleep(1000);
                        break;
                }

                if (info.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                {
                    _task = null;
                    _sendParcel = null;
                    return;
                }
            }

            info.Response.NumberOfCompletedSubOperations++;
            info.Response.NumberOfRemainingSubOperations--;
            _lastSendParcelProgress = _sendParcel.CurrentProgressStep;
        }

        #endregion

        #region Private Helper Functions

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

        private QueryKey BuildQueryKey(DcmDataset requestIdentifiers)
        {
            OFCondition cond;
            QueryKey queryKey = new QueryKey();

            // TODO: shouldn't hard code the buffer length like this
            StringBuilder buf = new StringBuilder(1024);

            // TODO: Edit these when we need to expand the support of search parameters
            cond = requestIdentifiers.findAndGetOFString(Dcm.PatientId, buf);
            if (cond.good())
                queryKey.Add(DicomTag.PatientId, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.AccessionNumber, buf);
            if (cond.good())
                queryKey.Add(DicomTag.AccessionNumber, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.PatientsName, buf);
            if (cond.good())
                queryKey.Add(DicomTag.PatientsName, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyDate, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyDate, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyDescription, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyDescription, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.ModalitiesInStudy, buf);
            if (cond.good())
                queryKey.Add(DicomTag.ModalitiesInStudy, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyInstanceUID, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyInstanceUID, buf.ToString());

            return queryKey;
        }

        private int QueryDB(DcmDataset requestIdentifiers)
        {
            try
            {
                _resultIndex = 0;
                _queryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(BuildQueryKey(requestIdentifiers));
            }
            catch (Exception exception)
            {
                Platform.Log(exception);
                return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
            }

            // query success means query has completed
            if (_queryResults.Count == 0)
                return OffisDcm.STATUS_Success;

            return OffisDcm.STATUS_Pending;
        }

        private int GetNextQueryResult(DcmDataset responseIdentifiers)
        {
            if (_queryResults != null && _queryResults.Count == 0 || _resultIndex >= _queryResults.Count)
            {
                // End of the query results
                _resultIndex = 0;
                _queryResults = null;
                return OffisDcm.STATUS_Success;
            }

            QueryResult result = _queryResults[_resultIndex];

            // TODO: Edit these when we need to expand the list of supported return tags
            responseIdentifiers.clear();
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientId), result.PatientId);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientsName), result.PatientsName);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDate), result.StudyDate);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyTime), result.StudyTime);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDescription), result.StudyDescription);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.ModalitiesInStudy), result.ModalitiesInStudy);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.AccessionNumber), result.AccessionNumber);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);

            _resultIndex++;

            return OffisDcm.STATUS_Pending;
        }

        private int InsertSopInstance(string fileName)
        {
            DcmFileFormat file = new DcmFileFormat();

            try
            {
                OFCondition condition = file.loadFile(fileName);
                if (!condition.good())
                {
                    // there was an error reading the file, possibly it's not a DICOM file
                    return OffisDcm.STATUS_STORE_Error_CannotUnderstand;
                }

                DcmMetaInfo metaInfo = file.getMetaInfo();
                DcmDataset dataset = file.getDataset();

                if (ConfirmProcessableFile(metaInfo, dataset))
                {
                    IDicomPersistentStore dicomStore = DataAccessLayer.GetIDicomPersistentStore();
                    dicomStore.InsertSopInstance(metaInfo, dataset, fileName);
                    dicomStore.Flush();
                }
            }
            catch (Exception exception)
            {
                Platform.Log(exception);
                return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
            }

            return OffisDcm.STATUS_Success;
        }

        private int StartMove(InteropMoveScpCallbackInfo info)
        {
            if (_task == null || _task.IsRunning == false)
            {
                ApplicationEntity destinationAE = FindDestinationAE(info.Request.MoveDestination);
                if (destinationAE == null)
                    return OffisDcm.STATUS_MOVE_Failed_MoveDestinationUnknown;

                OFCondition cond;
                StringBuilder buf = new StringBuilder(1024);
                String studyInstanceUID = null;
                String studyDescription = null;

                cond = info.ResponseIdentifiers.findAndGetOFString(Dcm.StudyInstanceUID, buf);
                if (cond.good())
                    studyInstanceUID = buf.ToString();

                if (studyInstanceUID == null || studyInstanceUID.Length == 0)
                    return OffisDcm.STATUS_MOVE_Failed_UnableToProcess;

                cond = info.ResponseIdentifiers.findAndGetOFString(Dcm.StudyDescription, buf);
                if (cond.good())
                    studyDescription = buf.ToString();

                // Add it to send Queue
                ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle(LocalApplicationEntity.AETitle), new ListeningPort(LocalApplicationEntity.Port));
                _sendParcel = (SendParcel)DicomServicesLayer.GetISender(me).Send(new Uid(studyInstanceUID), destinationAE, studyDescription);
                _lastSendParcelProgress = 0;
                info.Response.NumberOfRemainingSubOperations = (ushort)_sendParcel.GetToSendObjectCount();

                // Start sending the parcel
                _task = new BackgroundTask(delegate(IBackgroundTaskContext context) { _sendParcel.StartSend(); }, false);
                _task.Run();
            }

            return OffisDcm.STATUS_Pending;
        }

        #endregion

    }
}
