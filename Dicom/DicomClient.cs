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
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);
                
                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, ae.AE, ae.Host, ae.Port);
                associationParameters.ConfigureForStudyRootQuery();

                T_ASC_Association association = network.CreateAssociation(associationParameters);

                DcmDataset cFindDataset = new DcmDataset();

                
                DcmLongString patientIDElement = new DcmLongString(new DcmTag(dcm.PatientID), false);
                DcmPersonName patientsNameElement = new DcmPersonName(new DcmTag(dcm.PatientsName), false);
                DcmCodeString queryRetrieveLevel = new DcmCodeString(new DcmTag(dcm.QueryRetrieveLevel), false);

                // other tags we want to retrieve
                DcmCodeString modalitesInStudy = new DcmCodeString(new DcmTag(dcm.ModalitiesInStudy), false);
                DcmLongString studyDescription = new DcmLongString(new DcmTag(dcm.StudyDescription), false);
                DcmDate studyDate = new DcmDate(new DcmTag(dcm.StudyDate), false);
                DcmTime studyTime = new DcmTime(new DcmTag(dcm.StudyTime), false);
                DcmShortString accessionNumber = new DcmShortString(new DcmTag(dcm.AccessionNumber), false);
                DcmUniqueIdentifier studyInstanceUid = new DcmUniqueIdentifier(new DcmTag(dcm.StudyInstanceUID), false);

                // set the required keys
                patientIDElement.putString(patientID.ToString());
                patientsNameElement.putString(patientsName.ToString());
                queryRetrieveLevel.putString("STUDY");

                // put the elements into the C-FIND request

                cFindDataset.insert(patientIDElement);
                cFindDataset.insert(patientsNameElement);
                cFindDataset.insert(queryRetrieveLevel);
                cFindDataset.insert(modalitesInStudy);
                cFindDataset.insert(studyDescription);
                cFindDataset.insert(studyDate);
                cFindDataset.insert(studyTime);
                cFindDataset.insert(accessionNumber);
                cFindDataset.insert(studyInstanceUid);
                
                /*
                // set the query keys
                cFindDataset.putAndInsertString(new DcmTag(dcm.PatientID), patientID.ToString());
                cFindDataset.putAndInsertString(new DcmTag(dcm.PatientsName), patientsName.ToString());
                cFindDataset.putAndInsertString(new DcmTag(dcm.QueryRetrieveLevel), "STUDY");

                // set the other tags we want to retrieve
                cFindDataset.putAndInsertString(new DcmTag(dcm.ModalitiesInStudy), "");
                cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDescription), "");
                cFindDataset.putAndInsertString(new DcmTag(dcm.StudyDate), "");
                cFindDataset.putAndInsertString(new DcmTag(dcm.StudyTime), "");
                cFindDataset.putAndInsertString(new DcmTag(dcm.AccessionNumber), "");
                cFindDataset.putAndInsertString(new DcmTag(dcm.StudyInstanceUID), "");
                */
                if (association.SendCFindStudyRootQuery(cFindDataset))
                    return new ReadOnlyQueryResultCollection(_queryResults);
                else
                    return null;
            }
            catch (DicomRuntimeApplicationException e)
            {
                throw new NetworkDicomException(OffisConditionParser.GetTextString(ae, e), e);
            }
        }

        public ReadOnlyQueryResultCollection Query(ApplicationEntity ae, Uid studyInstanceUid)
        {
            if (studyInstanceUid.CompareTo(new Uid("1.2.840.1.2.311432.43242.266")) == 0)
            {
                   
                throw new Exception("Not yet implemented");
            }
            else
            {
                throw new Exception("Not yet implemented");
            }
        }

        public void Retrieve(ApplicationEntity ae, Uid uid, System.String path)
        {
            OnSopInstanceReceivedEvent(new SopInstanceReceivedEventArgs());
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
                    }
                }
            }

            private QueryCallbackHelperDelegate _queryCallbackHelperDelegate = null;
            private DicomClient _parent = null;
        }

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

    }
}
