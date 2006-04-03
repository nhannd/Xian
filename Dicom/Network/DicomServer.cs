namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using ClearCanvas.Dicom.OffisWrapper;
    using MySR = ClearCanvas.Dicom.SR;

    public class DicomServer
    {
        public DicomServer(ApplicationEntity ownAEParameters, String saveDirectory)
        {
            SocketManager.InitializeSockets();
            _myOwnAE = ownAEParameters;
            _state = ServerState.STOPPED;
            _stopRequestedFlag = false;
            _saveDirectory = DicomHelper.NormalizeDirectory(saveDirectory);
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
        public static void SetConnectionTimeout(Int16 timeout)
        {
            if (timeout < 1)
                throw new System.ArgumentOutOfRangeException("timeout", MySR.ExceptionDicomConnectionTimeoutOutOfRange);
            OffisDcm.SetConnectionTimeout(timeout);
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
            }

            private T_ASC_Association _association;
            private ThreadList _threadList;
            private DicomServer _parent;
        }

        public Int16 MaximumAssociations
        {
            get { return 25; }
        }

        protected class ThreadList : IList<Thread>
        {
            public Int16 NumberOfAliveThreads
            {
                get
                {
                    Int16 count = 0;
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
        #endregion
    }
}
