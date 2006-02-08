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
    using ClearCanvas.Dicom;

    public class DicomClient
    {
        public event EventHandler<SopInstanceReceivedEventArgs> SopInstanceReceivedEvent;
        public event EventHandler<SeriesCompletedEventArgs> SeriesCompletedEvent;
        public event EventHandler<StudyCompletedEventArgs> StudyCompletedEvent;
        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceivedEvent;
        public event EventHandler<QueryCompletedEventArgs> QueryCompletedEvent;

        public DicomClient(ApplicationEntity ownAEParameters)
        {
            // this is a temporary hack to initialize the sockets layer
            // I haven't been able to find how to properly do this
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            _myOwnAE = ownAEParameters;

            _queryCallbackHelper = new QueryCallbackHelper(this);
            _queryResults = new QueryResultList();
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

        public ReadOnlyQueryResultCollection Query(ApplicationEntity ae, PatientID patientID, PatientsName patientsName)
        {
            InitializeQueryState();

            DcmDataset cFindDataset = new DcmDataset();
            CreateStandardCFindDataset(ref cFindDataset);

            // set the specific query keys
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientID), patientID.ToString());
            cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsName), patientsName.ToString());

            ReadOnlyQueryResultCollection results = Query(ae, cFindDataset);
            FireConditionalQueryCompletedEvent(results);
            return results;
        }

        public ReadOnlyQueryResultCollection Query(ApplicationEntity ae, Uid studyInstanceUid)
        {
            InitializeQueryState();

            DcmDataset cFindDataset = new DcmDataset();
            CreateStandardCFindDataset(ref cFindDataset);

            // set the specific query for study instance uid
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), studyInstanceUid.ToString());

            ReadOnlyQueryResultCollection results = Query(ae, cFindDataset);
            FireConditionalQueryCompletedEvent(results);
            return results;
        }

        protected ReadOnlyQueryResultCollection Query(ApplicationEntity ae, DcmDataset cFindDataset)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, ae.AE, ae.Host, ae.Port);
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
                throw new NetworkDicomException(OffisConditionParser.GetTextString(ae, e), e);
            }
        }

        protected void CreateStandardCFindDataset(ref DcmDataset cFindDataset)
        {
            // set the Query Retrieve Level
            cFindDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "STUDY");

            // set the other tags we want to retrieve
            cFindDataset.putAndInsertString(new DcmTag(dcm.ModalitiesInStudy), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDescription), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDate), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyTime), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.AccessionNumber), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), "");
            cFindDataset.putAndInsertString(new DcmTag(dcm.SpecificCharacterSet), "");
        }

        protected void InitializeQueryState()
        {
            _queryResults.Clear();
        }

        protected void FireConditionalQueryCompletedEvent(ReadOnlyQueryResultCollection results)
        {
            if (null != results)
            {
                QueryCompletedEventArgs args = new QueryCompletedEventArgs(results);
                OnQueryCompletedEvent(args);
            }
        }

        public void Retrieve(ApplicationEntity serverAE, Uid studyInstanceUid, System.String saveDirectory)
        {
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
                message.AppendFormat(ClearCanvas.Dicom.SR.ExceptionDicomSaveDirectoryDoesNotExist, normalizedSaveDirectory.ToString());

                throw new System.ArgumentException(message.ToString(), "saveDirectory");
            }
            
            DcmDataset cMoveDataset = new DcmDataset();

            // set the specific query for study instance uid
            cMoveDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), studyInstanceUid.ToString());

            Retrieve(serverAE, cMoveDataset, normalizedSaveDirectory.ToString());
            
            // fire event to indicate successful retrieval
            return;
        }

        protected void Retrieve(ApplicationEntity serverAE, DcmDataset cMoveDataset, System.String saveDirectory)
        {
            try
            {
                OffisDcm.SetConnectionTimeout(600);

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

            private QueryCallbackHelperDelegate _queryCallbackHelperDelegate = null;
            private DicomClient _parent = null;
        }

        #region Private members
        private QueryCallbackHelper _queryCallbackHelper = null;
        private QueryResultList _queryResults = null;
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
