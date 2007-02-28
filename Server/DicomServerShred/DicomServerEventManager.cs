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

namespace ClearCanvas.Server.DicomServerShred
{
    public partial class DicomServerEventManager
    {
        private ApplicationEntity _myApplicationEntity;
        private string _storageDirectory;
        private ClearCanvas.Dicom.Network.DicomServer _dicomServer;

        // Used by CFindScp
        private Dictionary<DicomSessionKey, DicomQuerySession> _querySessionDictionary;
        private object _querySessionLock = new object();

        // Used by CMoveScp to keep track of the sub-CStore progerss
        private Dictionary<DicomSessionKey, DicomMoveSession> _moveSessionDictionary;
        private object _moveSessionLock = new object();

        public DicomServerEventManager(string hostName, string aeTitle, int port, string storageDirectory)
        {
            _myApplicationEntity = new ApplicationEntity(new HostName(hostName), new AETitle(aeTitle), new ListeningPort(port));
            _storageDirectory = storageDirectory;

            _querySessionDictionary = new Dictionary<DicomSessionKey, DicomQuerySession>();
            _moveSessionDictionary = new Dictionary<DicomSessionKey, DicomMoveSession>();
        }

        public void StartServer()
        {
            try
            {
                // Create storage directory
                if (!Directory.Exists(_storageDirectory))
                    Directory.CreateDirectory(_storageDirectory);

                _dicomServer = new ClearCanvas.Dicom.Network.DicomServer(_myApplicationEntity, _storageDirectory);

                _dicomServer.FindScpEvent += OnFindScpEvent;
                _dicomServer.FindScpProgressEvent += OnFindScpProgressEvent;
                _dicomServer.StoreScpBeginEvent += OnStoreScpBeginEvent;
                _dicomServer.StoreScpProgressEvent += OnStoreScpProgressEvent;
                _dicomServer.StoreScpEndEvent += OnStoreScpEndEvent;
                _dicomServer.MoveScpBeginEvent += OnMoveScpBeginEvent;
                _dicomServer.MoveScpProgressEvent += OnMoveScpProgressEvent;

                _dicomServer.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopServer()
        {
            try
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
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Properties

        public string HostName
        {
            get { return _myApplicationEntity.Host; }
        }

        public string AETitle
        {
            get { return _myApplicationEntity.AE; }
        }

        public int Port
        {
            get { return _myApplicationEntity.Port; }
        }

        public string SaveDirectory
        {
            get { return _storageDirectory; }
        }

        public bool IsServerRunning
        {
            get { return (_dicomServer == null ? false : _dicomServer.IsRunning); }
        }

        #endregion

        #region DicomServer FindScp Event Handlers

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

        #endregion

        #region DicomServer MoveScp Event Handlers

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

        #region DicomServer StoreScp Event Handlers

        private void OnStoreScpBeginEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            //WCF TODO: Notify ActivityService via WCF, the receiving of a SOP has begun
        }

        private void OnStoreScpProgressEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            //WCF TODO: Notify ActivityService via WCF, update the SOP receiving status.  This will happen quite frequent
        }

        private void OnStoreScpEndEvent(object sender, DicomServerEventArgs e)
        {
            InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
            if (info == null)
                return;

            //WCF TODO: Notify ActivityService via WCF, a SOP Instance has been stored successfully

            info.DimseStatus = (ushort)OffisDcm.STATUS_Success;
        }

        #endregion

        #region FindScp helper functions

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
                // Query DB for results
                ReadOnlyQueryResultCollection queryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(BuildQueryKey(requestIdentifiers));
                if (queryResults.Count == 0)
                    return OffisDcm.STATUS_Success;

                // Remember the query results for this session.  The DicomServer will call back to get query results
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
            DicomQuerySession querySession;

            try
            {
                lock (_querySessionLock)
                {
                    querySession = _querySessionDictionary[key];
                    if (querySession.CurrentIndex >= querySession.QueryResults.Count)
                    {
                        // If all the results had been retrieved, remove this query session from dictionary
                        _querySessionDictionary.Remove(key);
                        return OffisDcm.STATUS_Success;
                    }
                }

                // Otherwise, return the next query result based on the current query index
                result = querySession.QueryResults[querySession.CurrentIndex];
                querySession.CurrentIndex++;
            }
            catch (KeyNotFoundException)
            {
                // if key is not found, we return STATUS_Success anyway.  It means CFind has completed successfully
                return OffisDcm.STATUS_Success;
            }
            catch (Exception exception)
            {
                Platform.Log(exception, LogLevel.Error);
                return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
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

        #endregion

        #region MoveScp helper functions

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

        private int StartMove(DicomSessionKey key, InteropMoveScpCallbackInfo info)
        {
            lock (_moveSessionLock)
            {
                // Check if a duplicate move request is currently in progress
                if (_moveSessionDictionary.ContainsKey(key))
                {
                    DicomMoveSession storeSession = _moveSessionDictionary[key];
                    if (storeSession.Task != null && storeSession.Task.IsRunning)
                        return OffisDcm.STATUS_Success;
                }
            }

            //WCF TODO: Get ApplicationEntity info from service
            DicomServerTree serverTree = new DicomServerTree();
            ApplicationEntity destinationAE = FindDestinationAE(info.Request.MoveDestination, serverTree.MyServerGroup.ChildServers);
            if (destinationAE == null)
                return OffisDcm.STATUS_MOVE_Failed_MoveDestinationUnknown;

            String studyInstanceUID = GetTag(info.RequestIdentifiers, Dcm.StudyInstanceUID);
            String studyDescription = GetTag(info.RequestIdentifiers, Dcm.StudyDescription);
            if (studyInstanceUID == null || studyInstanceUID.Length == 0)
                return OffisDcm.STATUS_MOVE_Failed_UnableToProcess;

            //WCF TODO: notify ActivityService via WCF, and save the job number in the parcel's GUID
            SendParcel sendParcel = new SendParcel(_myApplicationEntity, destinationAE, studyDescription);
            sendParcel.Include(new Uid(studyInstanceUID));

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

        private string GetTag(DcmDataset dataSet, DcmTagKey tagKey)
        {
            // TODO: shouldn't hard code the buffer length like this
            StringBuilder buf = new StringBuilder(1024);

            OFCondition cond = dataSet.findAndGetOFString(tagKey, buf);
            if (cond.good())
                return buf.ToString();

            return "";
        }

    }
}
