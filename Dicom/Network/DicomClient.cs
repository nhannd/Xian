namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;    
    using System.Text;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom.OffisWrapper;
    using ClearCanvas.Dicom;
    using MySR = ClearCanvas.Dicom.SR;
    using System.IO;

    /// <summary>
    /// Main entry point for DICOM networking functionality. Allows the client to 
    /// perform C-ECHO, C-FIND and C-MOVE commands. Both C-FIND and C-MOVE 
    /// implement the Study Root Query/Retrieve Information Model.
    /// </summary>
    public class DicomClient
    {
        /// <summary>
        /// Fires when a new SOP Instance has arrived and has been successfully
        /// written to the local filesystem.
        /// </summary>
        private event EventHandler<SopInstanceReceivedEventArgs> _sopInstanceReceivedEvent;
        /// <summary>
        /// Fires when a C-FIND result is received.
        /// </summary>
        private event EventHandler<QueryResultReceivedEventArgs> _queryResultReceivedEvent;
        /// <summary>
        /// Fires when the C-FIND query has completed and all results received.
        /// </summary>
        private event EventHandler<QueryCompletedEventArgs> _queryCompletedEvent;

        private event EventHandler<ObjectSendingProgressUpdatedEventArgs> _objectSendingProgressUpdatedEvent;

        // TODO:
        private event EventHandler<SeriesCompletedEventArgs> SeriesCompletedEvent;
        private event EventHandler<StudyCompletedEventArgs> StudyCompletedEvent;

        public event EventHandler<SopInstanceReceivedEventArgs> SopInstanceReceivedEvent
        {
            add { _sopInstanceReceivedEvent += value; }
            remove { _sopInstanceReceivedEvent -= value; }
        }

        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceivedEvent
        {
            add { _queryResultReceivedEvent += value; }
            remove { _queryResultReceivedEvent -= value; }
        }

        public event EventHandler<QueryCompletedEventArgs> QueryCompletedEvent
        {
            add { _queryCompletedEvent += value; }
            remove { _queryCompletedEvent -= value; }
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="ownAEParameters">The AE parameters of the DICOM client. That is,
        /// the user's AE parameters that will be passed to the server as the source
        /// of the DICOM commands, and will also become the destination for the receiving
        /// of DICOM data.</param>
        public DicomClient(ApplicationEntity ownAEParameters)
        {
            SocketManager.InitializeSockets();

            _myOwnAE = ownAEParameters;

            // TODO:
            // since we want the QueryCallbackHelper object to be able to 
            // modifier data that is instance-related in nature, i.e. 
            // query results, we need to make the callback install itself
            // when this instance is created. This is not thread-safe
            _queryCallbackHelper = new QueryCallbackHelper(this);

            // same goes for the store callback helper
            _storeCallbackHelper = new StoreCallbackHelper(this);

            _queryResults = new QueryResultList();

            SetConnectionTimeout(_timeout);
        }

        ~DicomClient()
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

        /// <summary>
        /// Verifies that a remote AE has an operational DICOM implementation using
        /// C-ECHO. Note that a successful verify does not necessarily mean that
        /// the remote AE will be able to perform any particular service, since 
        /// the implementation at this time verifies support for the Verification
        /// Service Class only.
        /// </summary>
        /// <param name="serverAE">The AE parameters of the remote AE server.</param>
        /// <returns></returns>
        public bool Verify(ApplicationEntity serverAE)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                using (network)
                {
                    T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);

                    using (associationParameters)
                    {
                        associationParameters.ConfigureForVerification();

                        T_ASC_Association association = network.CreateAssociation(associationParameters);

                        using (association)
                        {
                            if (association.SendCEcho(_cEchoRepeats))
                            {
                                association.Release();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (DicomRuntimeApplicationException e)
            {                
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        /// <summary>
        /// This variation on the function takes Patient ID and Patient's Name.
        /// </summary>
        /// <overloads>There are currently seven overloads of this function. Query the 
        /// remote AE to determine what studies the server contains using a C-FIND. This 
        /// implementation uses the Study Root Query/Retrieve Information Model.
        /// <example>
        /// <code>
        ///ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
        ///    new AETitle("CCNETTEST"), new ListeningPort(110));
        ///ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
        ///    new AETitle("CONQUESTSRV1"), new ListeningPort(5678));
        ///
        ///DicomClient dicomClient = new DicomClient(myOwnAEParameters);
        ///
        ///if (!dicomClient.Verify(serverAE))
        ///    throw new Exception("Target server is not running");
        ///
        ///ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientId(""), new PatientsName(""));
        ///
        ///Assert.IsTrue(results.Count > 0);
        ///
        ///foreach (QueryResult qr in results)
        ///{   
        ///    foreach (DicomTag dicomTag in qr.DicomTags)
        ///    {
        ///        Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
        ///    }
        ///
        ///    Console.WriteLine("Patient's Name: {0}", qr.PatientsName);
        ///    Console.WriteLine("Patien ID: {0}", qr.PatientId);
        ///}
        /// </code>
        /// </example>
        /// </overloads>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientId">Key for searching: the relevant Patient ID.</param>
        /// <param name="patientsName">Key for searching: the relevant Patient's Name.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId, PatientsName patientsName)
        {
            ReadOnlyQueryResultCollection results = Query(serverAE, patientId, patientsName, new Accession(""));
            return results;
        }

        /// <summary>
        /// This variation on the function takes Patient ID.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientId">Key for searching: the relevant Patient ID.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId)
        {
            ReadOnlyQueryResultCollection results = Query(serverAE, patientId, new PatientsName(""));
            return results;
        }

        /// <summary>
        /// This variation on the function takes Patient's Name.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientsName">Key for searching: the relevant Patient's Name.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientsName patientsName)
        {
            ReadOnlyQueryResultCollection results = Query(serverAE, new PatientId(""), patientsName);
            return results;
        }

        /// <summary>
        /// Overload of the Query method that accepts a Study Instance UID.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="studyInstanceUid">Key for searching: the relevant study's Study Instance UID.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, Uid studyInstanceUid)
        {
            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Study);

            // set the specific query for study instance uid
            cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), studyInstanceUid.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);

            GC.KeepAlive(cFindDataset);
            return results;
        }

        /// <summary>
        /// Overload of the Query method that accepts Patient ID and Accession Number.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientId">Key for searching: The relevant Patient ID.</param>
        /// <param name="accession">Key for searching: The relevant Accession Number.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId, Accession accession)
        {
            ReadOnlyQueryResultCollection results = Query(serverAE, patientId, new PatientsName(""), accession);
            return results;
        }

        /// <summary>
        /// Overload of the Query method that accepts Patient's Name and Accession Number.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientsName">Key for searching: The relevant Patient's Name.</param>
        /// <param name="accession">Key for searching: The relevant Accession Number.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientsName patientsName, Accession accession)
        {
            ReadOnlyQueryResultCollection results = Query(serverAE, new PatientId(""), patientsName, accession);
            return results;
        }

        /// <summary>
        /// This is currently the most generalized version of Query that other versions will call.
        /// Overload of the Query method that accepts Patient ID, Patient's Name and Accession Number.
        /// </summary>
        /// <param name="serverAE">AE parameters of the remote AE server.</param>
        /// <param name="patientId">Key for searching: The relevant Patient ID.</param>
        /// <param name="patientsName">Key for searching: The relevant Patient's Name.</param>
        /// <param name="accession">Key for searching: The relevant Accession Number.</param>
        /// <returns>A read-only version of the <see cref="QueryResultCollection">QueryResultCollection</see>.
        /// Each C-FIND result is represented by one item in the collection, and it is possible to 
        /// enumerate over all the items.</returns>
        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId, PatientsName patientsName, Accession accession)
        {
            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Study);

            // set the specific query keys
            cFindDataset.putAndInsertString(new DcmTag(Dcm.PatientId), patientId.ToString());
            cFindDataset.putAndInsertString(new DcmTag(Dcm.PatientsName), patientsName.ToString());
            cFindDataset.putAndInsertString(new DcmTag(Dcm.AccessionNumber), accession.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);
            TriggerQueryCompletedEvent(results);

            GC.KeepAlive(cFindDataset);
            return results;
        }

        /// <summary>
        /// Query for the series that belong to a particular Study.
        /// </summary>
        /// <param name="serverAE">Server's AE parameters.</param>
        /// <param name="studyInstanceUid">Study Instance UID of the study that 
        /// the user wants to query the series for.</param>
        /// <returns>A read-only collection of query results. Successful results
        /// includes the Study UID, the Series UID, the Series Description and
        /// the Series Number.</returns>
        public ReadOnlyQueryResultCollection QuerySeries(ApplicationEntity serverAE, Uid studyInstanceUid)
        {
            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Series);

            // set the specific query keys
            cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), studyInstanceUid.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);
            TriggerQueryCompletedEvent(results);

            GC.KeepAlive(cFindDataset);
            return results;
        }

        /// <summary>
        /// Performs a DICOM retrieve using C-MOVE with the Study Root Query/Retrieve Information Model.
        /// A DICOM listener will automatically be created to receive the incoming DICOM data. The listener's
        /// AE parameters are defined by the ApplicationEntity used in the construction of the 
        /// <see cref="DicomClient">DicomClient</see>.
        /// </summary>
        /// <param name="serverAE">AE parameters of the server who will provided the C-MOVE service.</param>
        /// <param name="studyInstanceUid">Study Instance UID of the study to be retrieved.</param>
        /// <param name="saveDirectory">A path to a directory on the local filesystem that will receive
        /// the incoming DICOM data objects.</param>
        /// <example>
        /// <code>
        /// ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
        ///     new AETitle("CCNETTEST"), new ListeningPort(4000));
        /// ApplicationEntity serverAE = new ApplicationEntity(new HostName("localhost"),
        ///     new AETitle("CONQUESTSRV1"), new ListeningPort(5678));
        ///
        /// DicomClient dicomClient = new DicomClient(myOwnAEParameters);
        ///
        /// if (!dicomClient.Verify(serverAE))
        ///     throw new Exception("Target server is not running");
        ///
        /// dicomClient.SopInstanceReceivedEvent += SopInstanceReceivedEventHandler;
        /// dicomClient.Retrieve(serverAE, new Uid("1.3.46.670589.5.2.10.2156913941.892665384.993397"), "C:\\temp\\");
        /// dicomClient.SopInstanceReceivedEvent -= SopInstanceReceivedEventHandler;
        /// </code>
        /// </example>
        public void Retrieve(ApplicationEntity serverAE, Uid studyInstanceUid, System.String saveDirectory)
        {
            string normalizedSaveDirectory = DicomHelper.NormalizeDirectory(saveDirectory);

            DcmDataset cMoveDataset = new DcmDataset();

            // set the specific query for study instance uid
            InitializeStandardCMoveDataset(ref cMoveDataset, QRLevel.Study);
            cMoveDataset.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), studyInstanceUid.ToString());

            Retrieve(serverAE, cMoveDataset, normalizedSaveDirectory.ToString(), true);

            // fire event to indicate successful retrieval
            return;
        }
        
        /// <summary>
        /// Retrieves a series from the server.
        /// </summary>
        /// <param name="serverAE">Server's AE parameters.</param>
        /// <param name="seriesInstanceUid">The Series Instance UID of the series that the
        /// user wants to retrieve.</param>
        /// <param name="saveDirectory">The path to where the incoming images are stored.</param>
        public void RetrieveSeries(ApplicationEntity serverAE, Uid seriesInstanceUid, System.String saveDirectory)
        {
            string normalizedSaveDirectory = DicomHelper.NormalizeDirectory(saveDirectory);

            DcmDataset cMoveDataset = new DcmDataset();

            // set the specific query for study instance uid
            InitializeStandardCMoveDataset(ref cMoveDataset, QRLevel.Series);
            cMoveDataset.putAndInsertString(new DcmTag(Dcm.SeriesInstanceUID), seriesInstanceUid.ToString());

            Retrieve(serverAE, cMoveDataset, normalizedSaveDirectory.ToString(),true);

            // fire event to indicate successful retrieval
            return;
        }

        public void Store(ApplicationEntity serverAE, String directory, Boolean recursivelyDescend)
        {
            String normalizedDirectory = DicomHelper.NormalizeDirectory(directory);

            string[] fileList = Directory.GetFiles(directory,
                "*",
                recursivelyDescend ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            Store(serverAE, fileList);
        }

        public void Store(ApplicationEntity serverAE, IEnumerable<String> files)
        {
            string[] sopClassUids = { };
            Store(serverAE, files, sopClassUids);
        }

        public void Store(ApplicationEntity serverAE, IEnumerable<String> files, IEnumerable<String> sopClassUids)
        {
            string[] transferSyntaxUids = { };
            Store(serverAE, files, sopClassUids, transferSyntaxUids);
        }

        public void Store(ApplicationEntity serverAE, IEnumerable<String> files, IEnumerable<String> sopClassUids, IEnumerable<String> transferSyntaxUids)
        {
            // TODO:
            if (null == files)
            {
                throw new System.ArgumentNullException("files cannot be null");
            }

            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                using (network)
                {
                    T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);

                    using (associationParameters)
                    {
                        #region Interop-specific code
                        // copy over file list
                        OFStringVector interopFilenameList = new OFStringVector();
                        foreach (String filename in files)
                        {
                            interopFilenameList.Add(filename);
                        }

                        // copy over sop class list
                        OFStringVector interopSopClassUid = new OFStringVector();
                        foreach (String sopClass in sopClassUids)
                        {
                            interopSopClassUid.Add(sopClass);
                        }

                        // copy over syntax list
                        OFStringVector interopTransferSyntaxUid = new OFStringVector();
                        foreach (String syntax in transferSyntaxUids)
                        {
                            interopTransferSyntaxUid.Add(syntax);
                        }
                        #endregion

                        associationParameters.ConfigureForCStore(interopFilenameList, interopSopClassUid, interopTransferSyntaxUid);

                        T_ASC_Association association = network.CreateAssociation(associationParameters);

                        using (association)
                        {
                            if (association.SendCStore(interopFilenameList))
                                association.Release();

                            return;
                        }
                    }
                }
            }
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        #region Protected members

        /// <summary>
        /// Specifies the query level to be executed on a C-FIND (<see cref="Query">Query</see>) or C-MOVE
        /// (<see cref="Retrieve">Retrieve</see>) commands.
        /// </summary>
        protected enum QRLevel
        {
            /// <summary>
            /// Query at the Patient level, will return for example Patient's Name, Patient's Birthdate,
            /// one result per patient that matches search keys.
            /// </summary>
            Patient,
            /// <summary>
            /// Query at the Study level, will return for example, Study Date, Study Description, 
            /// one result per study that matches search keys.
            /// </summary>
            Study,
            /// <summary>
            /// Query at the Series level, will return for example, Modality, Series Description,
            /// one result per series that matches search keys.
            /// </summary>
            Series,
            /// <summary>
            /// Query at the Composite Object Instance level, will return for example Bits Allocated, 
            /// Planar Configuration, one result per SOP Instance that matches search keys.
            /// </summary>
            CompositeObjectInstance
        }

        /// <summary>
        /// The low-level version of Retrieve that just takes in a dataset that has already been
        /// properly initialized with the appropriate tags for the C-MOVE command.
        /// </summary>
        /// <param name="serverAE">The application entity that will serve our Retrieve request.</param>
        /// <param name="cMoveDataset">The dataset containing the parameters for this Retrieve.</param>
        /// <param name="saveDirectory">The path on the local filesystem that will store the
        /// DICOM objects that are received.</param>
        protected void Retrieve(ApplicationEntity serverAE, DcmDataset cMoveDataset, System.String saveDirectory, Boolean listenForSubAssociations)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_ACCEPTORREQUESTOR, _myOwnAE.Port, _timeout);

                using (network)
                {
                    T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);

                    using (associationParameters)
                    {
                        associationParameters.ConfigureForCMoveStudyRootQuery();

                        T_ASC_Association association = network.CreateAssociation(associationParameters);

                        using (association)
                        {
                            if (association.SendCMoveStudyRootQuery(cMoveDataset, network, saveDirectory, listenForSubAssociations))
                                association.Release();
                        }
                    }
                }

                return;
            }
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        /// <summary>
        /// The low-level version of Query that just takes in a dataset that has been properly 
        /// initialized and set to use tags that are appropriate for the C-FIND command.
        /// </summary>
        /// <param name="serverAE">The application entity that will serve our Query request.</param>
        /// <param name="cFindDataset">The dataset containing the parameters for this Query.</param>
        /// <returns>A read-only collection of matching results from the server in response to our Query.</returns>
        protected ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, DcmDataset cFindDataset)
        {
            InitializeQueryState();

            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                using (network)
                {
                    T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);

                    using (associationParameters)
                    {
                        associationParameters.ConfigureForStudyRootQuery();

                        T_ASC_Association association = network.CreateAssociation(associationParameters);

                        using (association)
                        {
                            if (association.SendCFindStudyRootQuery(cFindDataset))
                            {
                                association.Release();
                                return new ReadOnlyQueryResultCollection(_queryResults);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        /// <summary>
        /// Initializes a dataset with the required tags to ensure the C-FIND will work.
        /// </summary>
        /// <param name="cFindDataset">Dataset to be filled with certain required tags</param>
        /// <param name="level">Query/Retrieve level</param>
        protected static void InitializeStandardCFindDataset(ref DcmDataset cFindDataset, QRLevel level)
        {
            if (null == cFindDataset)
                throw new System.ArgumentNullException("level", MySR.ExceptionDicomFindDatasetNull);
            switch (level)
            {
                case QRLevel.Patient:
                    throw new System.ArgumentOutOfRangeException("level", MySR.ExceptionDicomPatientLevelQueryInvalid);
               
                case QRLevel.Study:
                    // set the Query Retrieve Level
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyDate), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyTime), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.AccessionNumber), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.PatientsName), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.PatientId), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.ModalitiesInStudy), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.StudyDescription), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SpecificCharacterSet), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.PatientsBirthDate), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.NumberOfStudyRelatedSeries), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.NumberOfStudyRelatedInstances), "");
                    break;
                case QRLevel.Series:
                    // set the Query Retrieve Level
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "SERIES");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.Modality), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SeriesInstanceUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SeriesDescription), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SeriesNumber), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.NumberOfSeriesRelatedInstances), "");
                    break;
                case QRLevel.CompositeObjectInstance:
                    // set the Query Retrieve Level
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "IMAGE");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SOPClassUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(Dcm.SOPInstanceUID), "");
                    break;
            }
        }
        
        /// <summary>
        /// Initializes a dataset with the required tags to ensure the C-MOVE will work.
        /// </summary>
        /// <param name="cMoveDataset">Dataset to be filled with certain required tags</param>
        /// <param name="level">Query/Retrieve level</param>
        protected static void InitializeStandardCMoveDataset(ref DcmDataset cMoveDataset, QRLevel level)
        {
            if (null == cMoveDataset)
                throw new System.ArgumentNullException("cMoveDataset", MySR.ExceptionDicomMoveDatasetNull);

            switch (level)
            {
                case QRLevel.Patient:
                    throw new System.ArgumentOutOfRangeException("level", MySR.ExceptionDicomPatientLevelQueryInvalid);
                    
                case QRLevel.Study:
                    // set the Query Retrieve Level
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), "");
                    break;
                case QRLevel.Series:
                    // set the Query Retrieve Level
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "SERIES");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.SeriesInstanceUID), "");
                    break;
                case QRLevel.CompositeObjectInstance:
                    // set the Query Retrieve Level
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "IMAGE");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(Dcm.SOPInstanceUID), "");
                    break;
            }
        }

        /// <summary>
        /// Instantiates a new list object, so that any old results will be left
        /// available (as long as it hasn't be GC'd).
        /// </summary>
        protected void InitializeQueryState()
        {
            _queryResults = new QueryResultList();
        }

        /// <summary>
        /// Invoke the event firing helper 
        /// </summary>
        /// <param name="results">Collection of query results or null if no results.
        /// were returned</param>
        protected void TriggerQueryCompletedEvent(ReadOnlyQueryResultCollection results)
        {
            QueryCompletedEventArgs args = new QueryCompletedEventArgs(results);
            OnQueryCompletedEvent(args);
        }
 
        protected void OnSopInstanceReceivedEvent(SopInstanceReceivedEventArgs e)
        {
            
            EventsHelper.Fire(_sopInstanceReceivedEvent, this, e);
        }

        protected void OnStudyCompletedEvent(StudyCompletedEventArgs e)
        {
            EventsHelper.Fire(StudyCompletedEvent, this, e);
        }

        protected void OnSeriesCompletedEvent(SeriesCompletedEventArgs e)
        {
            EventsHelper.Fire(SeriesCompletedEvent, this, e);
        }

        protected void OnQueryResultReceivedEvent(QueryResultReceivedEventArgs e)
        {
            EventsHelper.Fire(_queryResultReceivedEvent, this, e);
        }

        protected void OnQueryCompletedEvent(QueryCompletedEventArgs e)
        {
            EventsHelper.Fire(_queryCompletedEvent, this, e);
        }

        #endregion

        #region Private members
        
        class QueryCallbackHelper
        {
            public QueryCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _queryCallbackHelperDelegate = new QueryCallbackHelperDelegate(DefaultCallback);
                RegisterQueryCallbackHelper_OffisDcm(_queryCallbackHelperDelegate);
            }

            ~QueryCallbackHelper()
            {
                RegisterQueryCallbackHelper_OffisDcm(null);
            }

            public delegate void QueryCallbackHelperDelegate(IntPtr callbackData,
                                                             IntPtr request,
                                                             int responseCount,
                                                             IntPtr response,
                                                             IntPtr responseIdentifiers);

            [DllImport("OffisDcm", EntryPoint = "RegisterQueryCallbackHelper_OffisDcm")]
            public static extern void RegisterQueryCallbackHelper_OffisDcm(QueryCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr callbackData,
                                        IntPtr request,
                                        int responseCount,
                                        IntPtr response,
                                        IntPtr responseIdentifiers)
            {
                T_DIMSE_C_FindRQ outboundRequest = new T_DIMSE_C_FindRQ(request, false);
                T_DIMSE_C_FindRSP inboundResponse = new T_DIMSE_C_FindRSP(response, false);
                DcmDataset responseData = new DcmDataset(responseIdentifiers, false);

                if (DICOM_PENDING_STATUS(inboundResponse.DimseStatus) ||
                    DICOM_SUCCESS_STATUS(inboundResponse.DimseStatus))
                {
                    QueryResult queryResult = null;
                    DcmObject item = responseData.nextInContainer(null);
                    bool tagAvailable = (item != null);

                    // there actually is a result
                    if (tagAvailable)
                    {
                        queryResult = new QueryResult();

                        while (tagAvailable)
                        {
                            DcmElement element = OffisDcm.castToDcmElement(item);
                            if (null != element)
                            {
                                queryResult.Add(new DicomTag(element.getGTag(), element.getETag()), element.ToString());
                            }

                            item = responseData.nextInContainer(item);
                            tagAvailable = (item != null);
                        }

                        _parent._queryResults.Add(queryResult);

                        QueryResultReceivedEventArgs args = new QueryResultReceivedEventArgs(queryResult);
                        _parent.OnQueryResultReceivedEvent(args);
                    }
                }
            }

            private QueryCallbackHelperDelegate _queryCallbackHelperDelegate;
            private DicomClient _parent;
        }

        class StoreCallbackHelper
        {
            public StoreCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _storeCallbackHelperDelegate = new StoreCallbackHelperDelegate(DefaultCallback);
                RegisterStoreCallbackHelper_OffisDcm(_storeCallbackHelperDelegate);
            }

            ~StoreCallbackHelper()
            {
                RegisterStoreCallbackHelper_OffisDcm(null);
            }

            public delegate void StoreCallbackHelperDelegate(IntPtr interopStoreCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreCallbackHelper_OffisDcm")]
            public static extern void RegisterStoreCallbackHelper_OffisDcm(StoreCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreCallbackInfo)
            {
                InteropStoreCallbackInfo callbackInfo = new InteropStoreCallbackInfo(interopStoreCallbackInfo, false);
                string fileName = callbackInfo.FileName;
                DcmDataset imageDataset = callbackInfo.ImageDataset;

                // make a copy of the string, since the string passed in is 
                // allocated on the stack and will be gone very soon
                StringBuilder fileNameString = new StringBuilder();
                fileNameString.Append(fileName);

                SopInstanceReceivedEventArgs args = new SopInstanceReceivedEventArgs(fileNameString.ToString());
                _parent.OnSopInstanceReceivedEvent(args);
            }

            private StoreCallbackHelperDelegate _storeCallbackHelperDelegate;
            private DicomClient _parent;
        }

        class StoreScuCallbackHelper
        {
            public StoreScuCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _storeScuCallbackHelperDelegate = new StoreScuCallbackHelperDelegate(DefaultCallback);
                RegisterStoreScuCallbackHelper_OffisDcm(_storeScuCallbackHelperDelegate);
            }

            ~StoreScuCallbackHelper()
            {
                RegisterStoreScuCallbackHelper_OffisDcm(null);
            }

            public delegate void StoreScuCallbackHelperDelegate(IntPtr interopStoreScuCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScuCallbackHelper_OffisDcm")]
            public static extern void RegisterStoreScuCallbackHelper_OffisDcm(StoreScuCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreScuCallbackInfo)
            {
                InteropStoreScuCallbackInfo callbackInfo = new InteropStoreScuCallbackInfo(interopStoreScuCallbackInfo, false);
                T_DIMSE_C_StoreRQ request = callbackInfo.Request;
                T_DIMSE_StoreProgress progress = callbackInfo.Progress;


            }

            private StoreScuCallbackHelperDelegate _storeScuCallbackHelperDelegate;
            private DicomClient _parent;
        }

        private QueryCallbackHelper _queryCallbackHelper;
        private StoreCallbackHelper _storeCallbackHelper;
        private QueryResultList _queryResults;
        private ApplicationEntity _myOwnAE;
        private Int16 _timeout = 500;
        private int _defaultPDUSize = 16384;
        private int _cEchoRepeats = 7;

        private const UInt16 STATUS_Success = 0x0000;
        private const UInt16 STATUS_Pending = 0xff00;
        private const UInt16 STATUS_Warning = 0xb000;

        private static bool DICOM_PENDING_STATUS(UInt16 status) { return (((status)&STATUS_Pending) == 0xff00); }
        private static bool DICOM_WARNING_STATUS(UInt16 status) { return (((status)&STATUS_Warning) == 0xb000); }
        private static bool DICOM_SUCCESS_STATUS(UInt16 status) { return ( (status) == STATUS_Success ); }
        #endregion

    }
}
