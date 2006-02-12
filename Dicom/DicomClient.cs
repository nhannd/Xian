namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom.OffisWrapper;
    using ClearCanvas.Dicom.Exceptions;
    using ClearCanvas.Dicom.Data;
    using MySR = ClearCanvas.Dicom.SR;

    public class DicomClient
    {
        public event EventHandler<SopInstanceReceivedEventArgs> SopInstanceReceivedEvent;
        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceivedEvent;
        public event EventHandler<QueryCompletedEventArgs> QueryCompletedEvent;
        private event EventHandler<SeriesCompletedEventArgs> SeriesCompletedEvent;
        private event EventHandler<StudyCompletedEventArgs> StudyCompletedEvent;

        public DicomClient(ApplicationEntity ownAEParameters)
        {
            // this is a temporary hack to initialize the sockets layer
            // I haven't been able to find how to properly do this
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            _myOwnAE = ownAEParameters;

            // since we want the QueryCallbackHelper object to be able to 
            // modifier data that is instance-related in nature, i.e. 
            // query results, we need to make the callback install itself
            // when this instance is created. This is not thread-safe
            _queryCallbackHelper = new QueryCallbackHelper(this);

            // same goes for the store callback helper
            _storeCallbackHelper = new StoreCallbackHelper(this);

            _queryResults = new QueryResultList();
        }

        public static void SetConnectionTimeout(Int16 timeout)
        {
            if (timeout < 1)
                throw new System.ArgumentOutOfRangeException("timeout", MySR.ExceptionDicomConnectionTimeoutOutOfRange);
            OffisDcm.SetConnectionTimeout(timeout);
        }

        public bool Verify(ApplicationEntity ae)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, ae.AE, ae.Host, ae.Port);
                associationParameters.ConfigureForVerification();

                T_ASC_Association association = network.CreateAssociation(associationParameters);
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
            catch (DicomRuntimeApplicationException e)
            {                
                throw new NetworkDicomException(OffisConditionParser.GetTextString(ae, e), e);
            }
        }

        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId, PatientsName patientsName)
        {
            InitializeQueryState();

            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Study);

            // set the specific query keys
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientID), patientId.ToString());
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsName), patientsName.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);
            TriggerConditionalQueryCompletedEvent(results);
            return results;
        }

        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, Uid studyInstanceUid)
        {
            InitializeQueryState();

            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Study);

            // set the specific query for study instance uid
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), studyInstanceUid.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);
            TriggerConditionalQueryCompletedEvent(results);
            return results;
        }

        public ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, PatientId patientId, PatientsName patientsName, Accession accession)
        {
            InitializeQueryState();

            DcmDataset cFindDataset = new DcmDataset();
            InitializeStandardCFindDataset(ref cFindDataset, QRLevel.Study);

            // set the specific query keys
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientID), patientId.ToString());
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsName), patientsName.ToString());
            cFindDataset.putAndInsertString(new DcmTag(dcm.AccessionNumber), accession.ToString());

            ReadOnlyQueryResultCollection results = Query(serverAE, cFindDataset);
            TriggerConditionalQueryCompletedEvent(results);
            return results;
        }

        public void Retrieve(ApplicationEntity serverAE, Uid studyInstanceUid, System.String saveDirectory)
        {
			if (null == saveDirectory)
				throw new System.ArgumentNullException("saveDirectory", MySR.ExceptionDicomSaveDirectoryNull);

            // make sure that the path passed in has a trailing backslash 
            StringBuilder normalizedSaveDirectory = new StringBuilder();
            if (saveDirectory.EndsWith("\\"))
            {
                normalizedSaveDirectory.Append(saveDirectory);
            }
            else
            {
                normalizedSaveDirectory.AppendFormat("{0}\\", saveDirectory);
            }

            // contract check: existence of saveDirectory
            if (!System.IO.Directory.Exists(normalizedSaveDirectory.ToString()))
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat(MySR.ExceptionDicomSaveDirectoryDoesNotExist, normalizedSaveDirectory.ToString());

                throw new System.ArgumentException(message.ToString(), "saveDirectory");
            }

            DcmDataset cMoveDataset = new DcmDataset();

            // set the specific query for study instance uid
            InitializeStandardCMoveDataset(ref cMoveDataset, QRLevel.Study);
            cMoveDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), studyInstanceUid.ToString());

            Retrieve(serverAE, cMoveDataset, normalizedSaveDirectory.ToString());

            // fire event to indicate successful retrieval
            return;
        }

        #region Protected members

        protected enum QRLevel
        {
            Patient,
            Study,
            Series,
            CompositeObjectInstance
        }

        protected void Retrieve(ApplicationEntity serverAE, DcmDataset cMoveDataset, System.String saveDirectory)
        {
            try
            {
                SetConnectionTimeout(600);

                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_ACCEPTORREQUESTOR, _myOwnAE.Port, _timeout);

                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);
                associationParameters.ConfigureForCMoveStudyRootQuery();

                T_ASC_Association association = network.CreateAssociation(associationParameters);

                if (association.SendCMoveStudyRootQuery(cMoveDataset, network, saveDirectory))
                    association.Release();

                return;
            }
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        /// <summary>
        /// The low-level version of Query that just takes in a dataset that has been properly 
        /// initialized and set to use tags that are appropriate
        /// </summary>
        /// <param name="serverAE">The application entity that will serve our Query request</param>
        /// <param name="cFindDataset">The dataset containing the parameters for this Query</param>
        /// <returns>A collection of matching results from the server in response to our Query </returns>
        protected ReadOnlyQueryResultCollection Query(ApplicationEntity serverAE, DcmDataset cFindDataset)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, serverAE.AE, serverAE.Host, serverAE.Port);
                associationParameters.ConfigureForStudyRootQuery();

                T_ASC_Association association = network.CreateAssociation(associationParameters);

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
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(serverAE, e), e);
            }
        }

        /// <summary>
        /// Initializes a dataset with the required tags to ensure the C-FIND will work
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
                    cFindDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "STUDY");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDate), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.StudyTime), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.AccessionNumber), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsName), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.PatientID), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.ModalitiesInStudy), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDescription), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.SpecificCharacterSet), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsBirthDate), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.NumberOfStudyRelatedSeries), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.NumberOfStudyRelatedInstances), "");
                    break;
                case QRLevel.Series:
                    // set the Query Retrieve Level
                    cFindDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "SERIES");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(dcm.Modality), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.SeriesInstanceUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.NumberOfSeriesRelatedInstances), "");
                    break;
                case QRLevel.CompositeObjectInstance:
                    // set the Query Retrieve Level
                    cFindDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "IMAGE");

                    // set the other tags we want to retrieve
                    cFindDataset.putAndInsertString(new DcmTag(dcm.SOPClassUID), "");
                    cFindDataset.putAndInsertString(new DcmTag(dcm.SOPInstanceUID), "");
                    break;
            }
        }
        
        /// <summary>
        /// Initializes a dataset with the required tags to ensure the C-MOVE will work
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
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "STUDY");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), "");
                    break;
                case QRLevel.Series:
                    // set the Query Retrieve Level
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "SERIES");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.SeriesInstanceUID), "");
                    break;
                case QRLevel.CompositeObjectInstance:
                    // set the Query Retrieve Level
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "IMAGE");
                    // set the other tags we want to retrieve
                    cMoveDataset.putAndInsertString(new DcmTag(dcm.SOPInstanceUID), "");
                    break;
            }
        }

        /// <summary>
        /// Resets the contents of the query result collection, typically done before
        /// any query operations are invoked to ensure that the collection is empty
        /// </summary>
        protected void InitializeQueryState()
        {
            _queryResults.Clear();
        }

        /// <summary>
        /// Invoke the event firing helper if the query operation resulted in query results
        /// i.e. the query was successful
        /// </summary>
        /// <param name="results">Collection of query results or null if no results
        /// were returned</param>
        protected void TriggerConditionalQueryCompletedEvent(ReadOnlyQueryResultCollection results)
        {
            if (null != results)
            {
                QueryCompletedEventArgs args = new QueryCompletedEventArgs(results);
                OnQueryCompletedEvent(args);
            }
        }

        protected void OnSopInstanceReceivedEvent(SopInstanceReceivedEventArgs e)
        {
            
            EventsHelper.Fire(SopInstanceReceivedEvent, this, e);
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
            EventsHelper.Fire(QueryResultReceivedEvent, this, e);
        }

        protected void OnQueryCompletedEvent(QueryCompletedEventArgs e)
        {
            EventsHelper.Fire(QueryCompletedEvent, this, e);
        }

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
                    QueryResultDictionary queryResult = null;
                    DcmObject item = responseData.nextInContainer(null);
                    bool tagAvailable = (item != null);

                    // there actually is a result
                    if (tagAvailable)
                    {
                        queryResult = new QueryResultDictionary();

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

        #endregion

        #region Private members
        private QueryCallbackHelper _queryCallbackHelper;
        private StoreCallbackHelper _storeCallbackHelper;
        private QueryResultList _queryResults;
        private ApplicationEntity _myOwnAE;
        private int _timeout = 500;
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
