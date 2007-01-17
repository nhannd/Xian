namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using ClearCanvas.Dicom.OffisWrapper;
    using System.Runtime.InteropServices;    
    using MySR = ClearCanvas.Dicom.SR;
    using ClearCanvas.Common.Utilities;

    public class FindScpEventArgs : EventArgs
    {
        private QueryKey _queryKey;
        private ReadOnlyQueryResultCollection _queryResults;

        public FindScpEventArgs(QueryKey queryKey, ReadOnlyQueryResultCollection queryResults)
        {
            _queryKey = queryKey;
            _queryResults = queryResults;
        }

        public QueryKey QueryKey
        {
            get { return _queryKey; }
            set { _queryKey = value; }
        }

        public ReadOnlyQueryResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; }
        }
    }

    public class StoreScpEventArgs : EventArgs
    {
        private string _fileName;
        private DcmDataset _imageDataset;

        public StoreScpEventArgs(string fileName, DcmDataset imageDataSet)
        {
            _fileName = fileName;
            _imageDataset = imageDataSet;
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public DcmDataset ImageDataSet
        {
            get { return _imageDataset; }
            set { _imageDataset = value; }
        }
    }

    /// <summary>
    /// This class's implementation is not yet complete. Use at your own risk.
    /// </summary>
    public class DicomServer
    {
        private event EventHandler<FindScpEventArgs> _findScpEvent;
        private event EventHandler<StoreScpEventArgs> _storeScpEvent;
        public event EventHandler<FindScpEventArgs> FindScpEvent
        {
            add { _findScpEvent += value; }
            remove { _findScpEvent -= value; }
        }
        public event EventHandler<StoreScpEventArgs> StoreScpEvent
        {
            add { _storeScpEvent += value; }
            remove { _storeScpEvent -= value; }
        }
        protected void OnFindScpEvent(FindScpEventArgs e)
        {
            EventsHelper.Fire(_findScpEvent, this, e);
        }
        protected void OnStoreScpEvent(StoreScpEventArgs e)
        {
            EventsHelper.Fire(_storeScpEvent, this, e);
        }

        public DicomServer(ApplicationEntity ownAEParameters, String saveDirectory)
        {
            SocketManager.InitializeSockets();
            _myOwnAE = ownAEParameters;
            _state = ServerState.STOPPED;
            _stopRequestedFlag = false;
            _saveDirectory = DicomHelper.NormalizeDirectory(saveDirectory);

            _findScpCallbackHelper = new FindScpCallbackHelper(this);
            _storeScpCallbackHelper = new StoreScpCallbackHelper(this);
        }


        public void Start()
        {
            // TODO
            if (ServerState.INITIALIZING == State)
                throw new System.Exception("Server is already in the process of initialization: cannot start again.");

            if (ServerState.STARTED == State)
                throw new System.Exception("Server is already started: cannot start again");

            Thread t = new Thread(new ThreadStart(WaitForIncomingAssociations));
            t.IsBackground = true;
            t.Start();
        }

        public String SaveDirectory
        {
            get { return _saveDirectory; }
        }

        protected void WaitForIncomingAssociations()
        {
            State = ServerState.INITIALIZING;

            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_ACCEPTOR, _myOwnAE.Port, _timeout);

                State = ServerState.STARTED;

                do
                {
                    T_ASC_Association association =  network.AcceptAssociation(_myOwnAE.AE, _operationTimeout, _threadList.NumberOfAliveThreads, MaximumAssociations);

                    if (null != association)
                    {

                        // spawn a thread to process the association
                        AssociationProcessor processor = new AssociationProcessor(this, association, _threadList);
                        Thread t = new Thread(new ThreadStart(processor.Process));
                        t.IsBackground = true;
                        // put the thread into a data structure that keeps track of all the threads
                        _threadList.Add(t);

                        t.Start();
                    }

                    // association returned was null, this is the semantic meaning 
                    // that the timeout has expired, and we should check to see whether
                    // a stop request has occurred.
                    if (StopRequestedFlag)
                    {
                        // wait for all the child processing threads to complete
                        foreach (Thread t in _threadList)
                        {
                            if (t.IsAlive)
                                t.Join();
                        }
                        break;
                    }
                } while (true);

                State = ServerState.STOPPED;

            }
            catch (DicomRuntimeApplicationException e)
            {
                State = ServerState.STOPPED;
                throw new NetworkDicomException(OffisConditionParser.GetTextString(_myOwnAE, e), e);
            }
        }

        public void Stop()
        {
            // TODO
            if (ServerState.STARTED != State)
                throw new System.Exception("Cannot stop server while it has not yet been started.");

            StopRequestedFlag = true;
        }

        ~DicomServer()
        {
            SocketManager.DeinitializeSockets();
        }


        /// <summary>
        /// Set the overall connection timeout period in the underlying OFFIS DICOM
        /// library.
        /// </summary>
        /// <param name="timeout">Timeout period in seconds.</param>
        public static void SetGlobalConnectionTimeout(ushort timeout)
        {
            if (timeout < 1)
                throw new System.ArgumentOutOfRangeException("timeout", MySR.ExceptionDicomConnectionTimeoutOutOfRange);
            OffisDcm.SetGlobalConnectionTimeout(timeout);
        }

        public short MaximumAssociations
        {
            get { return 25; }
        }

        protected enum ServerState
        {
            STOPPED,
            INITIALIZING,
            STARTED
        }

        protected class AssociationProcessor
        {
            public AssociationProcessor(DicomServer parent, T_ASC_Association association, ThreadList threadList)
            {
                _parent = parent;
                _association = association;
                _threadList = threadList;
            }

            public void Process()
            {
                OFCondition cond = _association.ProcessServerCommands(-1, _parent.SaveDirectory);
                if (DicomHelper.CompareConditions(cond, OffisDcm.DUL_PEERREQUESTEDRELEASE))
                {
                    _association.RespondToReleaseRequest();
                }
                else
                {
                    try
                    {
                        bool releaseSuccessful = _association.Release();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            private T_ASC_Association _association;
            private ThreadList _threadList;
            private DicomServer _parent;
        }

        protected class ThreadList : IList<Thread>
        {
            public short NumberOfAliveThreads
            {
                get
                {
                    short count = 0;
                    foreach (Thread t in _threadList)
                    {
                        if (t.IsAlive)
                            count++;
                    }

                    return count;
                }
            }

            #region IList<Thread> Members

            public int IndexOf(Thread item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Insert(int index, Thread item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void RemoveAt(int index)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public Thread this[int index]
            {
                get
                {
                    throw new Exception("The method or operation is not implemented.");
                }
                set
                {
                    throw new Exception("The method or operation is not implemented.");
                }
            }

            #endregion

            #region ICollection<Thread> Members

            public void Add(Thread item)
            {
                lock (_monitorLock)
                {
                    _threadList.Add(item);
                }
            }

            public void Clear()
            {
                lock (_monitorLock)
                {
                    _threadList.Clear();
                }
            }

            public bool Contains(Thread item)
            {
                lock (_monitorLock)
                {
                    return _threadList.Contains(item);
                }
            }

            public void CopyTo(Thread[] array, int arrayIndex)
            {
                lock (_monitorLock)
                {
                    _threadList.CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get
                {
                    lock (_monitorLock)
                    {
                        return _threadList.Count;
                    }
                }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(Thread item)
            {
                lock (_monitorLock)
                {
                    return _threadList.Remove(item);
                }
            }

            #endregion

            #region IEnumerable<Thread> Members

            public IEnumerator<Thread> GetEnumerator()
            {
                return (_threadList as IEnumerable<Thread>).GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _threadList.GetEnumerator();
            }

            #endregion

            private object _monitorLock = new object();
            private List<Thread> _threadList = new List<Thread>();
        }

        class FindScpCallbackHelper
        {
            private FindScpCallbackHelper_QueryDBDelegate _findScpCallbackHelper_QueryDBDelegate;
            private FindScpCallbackHelper_GetNextResponseDelegate _findScpCallbackHelper_GetNextResponseDelegate;
            private DicomServer _parent;
            private ReadOnlyQueryResultCollection _queryResults;
            private int _resultIndex;

            public FindScpCallbackHelper(DicomServer parent)
            {
                _parent = parent;
                _queryResults = null;
                _resultIndex = 0;

                _findScpCallbackHelper_QueryDBDelegate = new FindScpCallbackHelper_QueryDBDelegate(QueryDBCallback);
                _findScpCallbackHelper_GetNextResponseDelegate = new FindScpCallbackHelper_GetNextResponseDelegate(GetNextResponseCallback);
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(_findScpCallbackHelper_QueryDBDelegate);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(_findScpCallbackHelper_GetNextResponseDelegate);
            }

            ~FindScpCallbackHelper()
            {
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(null);
            }

            public delegate void FindScpCallbackHelper_QueryDBDelegate(IntPtr interopFindScpCallbackInfo);
            public delegate void FindScpCallbackHelper_GetNextResponseDelegate(IntPtr interopFindScpCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterFindScpCallbackHelper_QueryDB_OffisDcm")]
            public static extern void RegisterFindScpCallbackHelper_QueryDB_OffisDcm(FindScpCallbackHelper_QueryDBDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm")]
            public static extern void RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(FindScpCallbackHelper_GetNextResponseDelegate callbackDelegate);

            public void QueryDBCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                InteropFindScpCallbackInfo interopFindScpCallbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);

                T_DIMSE_C_FindRQ request = interopFindScpCallbackInfo.Request;
                T_DIMSE_C_FindRSP response = interopFindScpCallbackInfo.Response;
                DcmDataset requestIdentifiers = interopFindScpCallbackInfo.RequestIdentifiers;

                try
                {
                    _resultIndex = 0;
                    _queryResults = null;

                    QueryKey queryKey = BuildQueryKey(requestIdentifiers);
                    FindScpEventArgs args = new FindScpEventArgs(queryKey, null);
                    _parent.OnFindScpEvent(args);
                    _queryResults = args.QueryResults;

                    response.MessageIDBeingRespondedTo = request.MessageID;
                    response.AffectedSOPClassUID = request.AffectedSOPClassUID;
                    if (_queryResults.Count == 0)
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                    else
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
                }
                catch
                {
                    response.DimseStatus = (ushort)OffisDcm.STATUS_FIND_Failed_UnableToProcess;
                }
            }

            public void GetNextResponseCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                InteropFindScpCallbackInfo interopFindScpCallbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);

                T_DIMSE_C_FindRQ request = interopFindScpCallbackInfo.Request;
                T_DIMSE_C_FindRSP response = interopFindScpCallbackInfo.Response;
                DcmDataset requestIdentifiers = interopFindScpCallbackInfo.RequestIdentifiers;
                DcmDataset responseIdentifiers = interopFindScpCallbackInfo.ResponseIdentifiers;

                try
                {
                    response.MessageIDBeingRespondedTo = request.MessageID;
                    response.AffectedSOPClassUID = request.AffectedSOPClassUID;

                    if (_queryResults.Count == 0 || _resultIndex >= _queryResults.Count)
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                    else
                    {
                        GetQueryResult(_resultIndex, responseIdentifiers);
                        _resultIndex++;
                    }
                }
                catch
                {
                    response.DimseStatus = (ushort)OffisDcm.STATUS_FIND_Failed_UnableToProcess;
                }
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

                return queryKey;
            }

            private void GetQueryResult(int index, DcmDataset responseIdentifiers)
            {
                QueryResult result = _queryResults[index];

                // TODO:  edit these when we need to expand the list of supported return tags
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
            }
        }

        class StoreScpCallbackHelper
        {
            private StoreScpCallbackHelperDelegate _storeScpCallbackHelperDelegate;
            private DicomServer _parent;

            public StoreScpCallbackHelper(DicomServer parent)
            {
                _parent = parent;

                _storeScpCallbackHelperDelegate = new StoreScpCallbackHelperDelegate(DefaultCallback);
                RegisterStoreCallbackHelper_OffisDcm(_storeScpCallbackHelperDelegate);
            }

            ~StoreScpCallbackHelper()
            {
                RegisterStoreCallbackHelper_OffisDcm(null);
            }

            public delegate void StoreScpCallbackHelperDelegate(IntPtr interopStoreCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreCallbackHelper_OffisDcm")]
            public static extern void RegisterStoreCallbackHelper_OffisDcm(StoreScpCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreCallbackInfoPointer)
            {
                InteropStoreCallbackInfo interopStoreCallbackInfo = new InteropStoreCallbackInfo(interopStoreCallbackInfoPointer, false);

                StoreScpEventArgs args = new StoreScpEventArgs(interopStoreCallbackInfo.FileName, interopStoreCallbackInfo.ImageDataset);
                _parent.OnStoreScpEvent(args);

            }
        }

        #region Private members
        private ServerState State
        {
            get
            {
                lock (_monitorLock)
                {
                    return _state;
                }
            }

            set
            {
                lock (_monitorLock)
                {
                    _state = value;
                }
            }
        }
        private bool StopRequestedFlag
        {
            get
            {
                lock (_monitorLock)
                {
                    return _stopRequestedFlag;
                }
            }

            set
            {
                lock (_monitorLock)
                {
                    _stopRequestedFlag = value;
                }
            }
        }

        private ApplicationEntity _myOwnAE;
        private int _timeout = 500;
        private int _operationTimeout = 5;
        private ServerState _state;
        private bool _stopRequestedFlag;
        private object _monitorLock = new object();
        private ThreadList _threadList = new ThreadList();
        private String _saveDirectory;
        private FindScpCallbackHelper _findScpCallbackHelper;
        private StoreScpCallbackHelper _storeScpCallbackHelper;

        #endregion
    }
}
