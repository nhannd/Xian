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
    public class DicomSessionKey
    {
        private string _aeTitle;
        private string _hostName;
        private int _messageID;

        public DicomSessionKey(string aeTitle, string hostName, int messageID)
        {
            _aeTitle = aeTitle;
            _hostName = hostName;
            _messageID = messageID;
        }

        #region Properties

        public string AETitle
        {
            get { return _aeTitle; }
        }

        public string HostName
        {
            get { return _hostName; }
        }

        public int MessageID
        {
            get { return _messageID; }
        }

        #endregion

        #region Override functions

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            DicomSessionKey key = obj as DicomSessionKey;
            if (key == obj)
                return true;

            if (_aeTitle != key._aeTitle)
                return false;

            if (_hostName != key._hostName)
                return false;

            if (_messageID != key._messageID)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _aeTitle.GetHashCode() + _hostName.GetHashCode() + _messageID.GetHashCode();
        }

        #endregion
    }

    public class DicomQuerySession
    {
        ReadOnlyQueryResultCollection _queryResults;
        int _currentIndex;

        public DicomQuerySession(ReadOnlyQueryResultCollection queryResults)
        {
            _queryResults = queryResults;
            _currentIndex = 0;
        }

        #region Properties

        public ReadOnlyQueryResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        #endregion
    }

    public class DicomMoveSession
    {
        private SendParcel _parcel;
        private BackgroundTask _task;
        private int _progress = 0;

        public DicomMoveSession(SendParcel parcel, BackgroundTask task)
        {
            _parcel = parcel;
            _task = task;
            _progress = 0;
        }

        #region Properties

        public SendParcel Parcel
        {
            get { return _parcel; }
            set { _parcel = value; }
        }

        public BackgroundTask Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        #endregion
    }

    public class DicomServerEventManager
    {
        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;

        // Used by CFindScp
        private Dictionary<DicomSessionKey, DicomQuerySession> _querySessionDictionary;
        private object _querySessionLock = new object();

        // Used by CMoveScp to keep track of the sub-CStore progerss
        private Dictionary<DicomSessionKey, DicomMoveSession> _moveSessionDictionary;
        private object _moveSessionLock = new object();

        // Used by CStoreScp, TODO: this will be move to a database later on
        private Queue<string> _sopInstanceStoreQueue;
        private object _storeSessionLock = new object();
        private BackgroundTask _storeQueueInsertionTask;

        public DicomServerEventManager()
        {
            _querySessionDictionary = new Dictionary<DicomSessionKey, DicomQuerySession>();
            _moveSessionDictionary = new Dictionary<DicomSessionKey, DicomMoveSession>();
            _sopInstanceStoreQueue = new Queue<string>();
            _storeQueueInsertionTask = new BackgroundTask(delegate(IBackgroundTaskContext context) { InsertQueueToDB(context); }, true);
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
            _storeQueueInsertionTask.Run();
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

                _querySessionDictionary.Clear();
                _moveSessionDictionary.Clear();
                _storeQueueInsertionTask.RequestCancel();
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

            DicomSessionKey key = new DicomSessionKey(info.CallingAETitle, info.CallingPresentationAddress, info.Request.MessageID);
            info.Response.DimseStatus = (ushort) QueryDB(key, info.RequestIdentifiers);
        }

        private void OnFindScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            DicomSessionKey key = new DicomSessionKey(info.CallingAETitle, info.CallingPresentationAddress, info.Request.MessageID);
            info.Response.DimseStatus = (ushort)GetNextQueryResult(key, info.ResponseIdentifiers);
        }

        private void OnStoreScpBeginEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

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

            // A new SOP Instance has been written successfully to the disk, update the queue
            lock (_storeSessionLock)
            {
                _sopInstanceStoreQueue.Enqueue(info.FileName);
            }

            info.DimseStatus = (ushort) OffisDcm.STATUS_Success;

            //SopInstanceReceivedEventArgs arg = new SopInstanceReceivedEventArgs(...)
            //EventsHelper.Fire(_sopInstanceReceivedEvent, this, arg);
        }

        private void OnMoveScpBeginEvent(object sender, DicomServerEventArgs e)
        {
            InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            DicomSessionKey key = new DicomSessionKey(info.CallingAETitle, info.CallingPresentationAddress, info.Request.MessageID);

            // Start the Query
            info.Response.DimseStatus = (ushort)QueryDB(key, info.RequestIdentifiers);

            // Move all return results
            while (info.Response.DimseStatus == (ushort) OffisDcm.STATUS_Pending)
            {
                info.Response.DimseStatus = (ushort)GetNextQueryResult(key, info.ResponseIdentifiers);
                if (info.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                    break;

                info.Response.DimseStatus = (ushort)StartMove(key, info);
            }
        }

        private void OnMoveScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            DicomSessionKey key = new DicomSessionKey(info.CallingAETitle, info.CallingPresentationAddress, info.Request.MessageID);
            info.Response.DimseStatus = (ushort)UpdateMoveProgress(key, info.Response);
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

        private int QueryDB(DicomSessionKey key, DcmDataset requestIdentifiers)
        {
            try
            {
                ReadOnlyQueryResultCollection queryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(BuildQueryKey(requestIdentifiers));
                if (queryResults.Count == 0)
                    return OffisDcm.STATUS_Success;

                // Add it to session query results
                lock (_querySessionLock)
                {
                    _querySessionDictionary[key] = new DicomQuerySession(queryResults);
                }
            }
            catch (Exception exception)
            {
                Platform.Log(exception);
                return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
            }

            return OffisDcm.STATUS_Pending;
        }

        private int GetNextQueryResult(DicomSessionKey key, DcmDataset responseIdentifiers)
        {
            QueryResult result;

            try
            {
                DicomQuerySession querySession;

                lock (_querySessionLock)
                {
                    querySession = _querySessionDictionary[key];

                    if (querySession.CurrentIndex >= querySession.QueryResults.Count)
                    {
                        _querySessionDictionary.Remove(key);
                        return OffisDcm.STATUS_Success;
                    }
                }

                result = querySession.QueryResults[querySession.CurrentIndex];
                querySession.CurrentIndex++;
            }
            catch // (KeyNotFoundException exception)
            {
                // return STATUS_Success means CFind completed successfully, even if key is not found
                return OffisDcm.STATUS_Success;
            }

            // Edit these when we need to expand the list of supported return tags
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

            return OffisDcm.STATUS_Pending;
        }

        private void InsertQueueToDB(IBackgroundTaskContext context)
        {
            IDicomPersistentStore dicomStore = DataAccessLayer.GetIDicomPersistentStore();
            int flushSize = 10;
            int sleepTime = 1000;
            String fileName = null;

            while (context.CancelRequested == false)
            {
                // Inner loop to insert to DB
                while (_sopInstanceStoreQueue.Count > 0)
                {
                    lock (_storeSessionLock)
                    {
                        fileName = _sopInstanceStoreQueue.Dequeue();
                    }

                    InsertSopInstance(dicomStore, fileName);

                    if (context.CancelRequested)
                    {
                        dicomStore.Flush();
                        context.Cancel();
                        return;
                    }

                    // Flush to DB regularly
                    if (dicomStore.GetCachedStudiesCount() > flushSize)
                        dicomStore.Flush();
                }

                // Flush any remaining cached studies
                if (dicomStore.GetCachedStudiesCount() > 0)
                    dicomStore.Flush();

                System.Threading.Thread.Sleep(sleepTime);            
            }            
            
            context.Complete(null);
        }

        private void InsertSopInstance(IDicomPersistentStore dicomStore, String fileName)
        {
            if (dicomStore == null || fileName == null)
                return;

            DcmFileFormat file = new DcmFileFormat();

            try
            {
                OFCondition condition = file.loadFile(fileName);
                if (!condition.good())
                {
                    // there was an error reading the file, possibly it's not a DICOM file
                    Platform.Log(condition.text());
                }

                DcmMetaInfo metaInfo = file.getMetaInfo();
                DcmDataset dataset = file.getDataset();

                if (ConfirmProcessableFile(metaInfo, dataset))
                {
                    String storageFileName = MoveTempFileToStorage(fileName, dataset);
                    dicomStore.InsertSopInstance(metaInfo, dataset, storageFileName);
                }
            }
            catch (Exception exception)
            {
                Platform.Log(exception);
                return;
            }
        }

        private String MoveTempFileToStorage(String tempFileName, DcmDataset imageDataSet)
        {
            String fileName;

            try
            {
                OFCondition cond;
                StringBuilder buf = new StringBuilder(1024);
                String studyInstanceUID = null;
                String seriesInstanceUID = null;
                String sopInstanceUID = null;

                cond = imageDataSet.findAndGetOFString(Dcm.StudyInstanceUID, buf);
                if (cond.good())
                    studyInstanceUID = buf.ToString();
                cond = imageDataSet.findAndGetOFString(Dcm.SeriesInstanceUID, buf);
                if (cond.good())
                    seriesInstanceUID = buf.ToString();
                cond = imageDataSet.findAndGetOFString(Dcm.SOPInstanceUID, buf);
                if (cond.good())
                    sopInstanceUID = buf.ToString();

                String filePath = String.Format("{0}\\{1}\\{2}", this.SaveDirectory, studyInstanceUID, seriesInstanceUID);
                Directory.CreateDirectory(filePath);

                fileName = String.Format("{0}\\{1}.dcm", filePath, sopInstanceUID);
                if (File.Exists(fileName))
                    File.Delete(fileName);
                File.Move(tempFileName, fileName);
            }
            catch
            {
                fileName = tempFileName;
            }

            return fileName;
        }

        private int StartMove(DicomSessionKey key, InteropMoveScpCallbackInfo info)
        {
            lock (_moveSessionLock)
            {
                if (_moveSessionDictionary.ContainsKey(key))
                {
                    DicomMoveSession storeSession = _moveSessionDictionary[key];
                    if (storeSession.Task != null && storeSession.Task.IsRunning)
                    {
                        // Another move is already in session... do something
                        return OffisDcm.STATUS_Success;
                    }
                }
            }

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
            SendParcel sendParcel = (SendParcel)DicomServicesLayer.GetISender(me).Send(new Uid(studyInstanceUID), destinationAE, studyDescription);
            BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context) { sendParcel.StartSend(); }, false);
            lock (_moveSessionLock)
            {
                _moveSessionDictionary[key] = new DicomMoveSession(sendParcel, task);
            }

            info.Response.NumberOfRemainingSubOperations = (ushort)sendParcel.GetToSendObjectCount();
            task.Run();

            return OffisDcm.STATUS_Pending;
        }

        private int UpdateMoveProgress(DicomSessionKey key, T_DIMSE_C_MoveRSP response)
        {
            int status = OffisDcm.STATUS_Success;
            if (_moveSessionDictionary.ContainsKey(key) == false)
                return status;

            DicomMoveSession storeSession = _moveSessionDictionary[key];

            // Keep the thread here and only reutrn CMoveRSP when there's an error or progress update
            while (storeSession.Progress == storeSession.Parcel.CurrentProgressStep)
            {
                switch (storeSession.Parcel.GetState())
                {
                    case ParcelTransferState.Completed:
                        response.NumberOfCompletedSubOperations = (ushort)storeSession.Parcel.GetToSendObjectCount();
                        response.NumberOfRemainingSubOperations = 0;
                        status = OffisDcm.STATUS_Success;
                        break;
                    case ParcelTransferState.Cancelled:
                    case ParcelTransferState.CancelRequested:
                        response.NumberOfWarningSubOperations++;
                        response.NumberOfRemainingSubOperations--;
                        status = OffisDcm.STATUS_MOVE_Cancel_SubOperationsTerminatedDueToCancelIndication;
                        break;
                    case ParcelTransferState.Error:
                    case ParcelTransferState.Unknown:
                        response.NumberOfFailedSubOperations++;
                        response.NumberOfRemainingSubOperations--;
                        status = OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                        break;
                    case ParcelTransferState.InProgress:
                    case ParcelTransferState.Paused:
                    case ParcelTransferState.PauseRequested:
                    case ParcelTransferState.Pending:
                    default:
                        status = OffisDcm.STATUS_Pending;
                        System.Threading.Thread.Sleep(1000);
                        break;
                }

                if (status != OffisDcm.STATUS_Pending)
                {
                    _moveSessionDictionary.Remove(key);
                    return status;
                }
            }

            response.NumberOfCompletedSubOperations++;
            response.NumberOfRemainingSubOperations--;
            storeSession.Progress = storeSession.Parcel.CurrentProgressStep;

            return status;
        }

        #endregion

    }
}
