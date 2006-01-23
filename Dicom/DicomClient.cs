namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Sockets;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom.Network.OffisWrapper;
    using ClearCanvas.Dicom.Exceptions;

    public class DicomClient
    {
        public event EventHandler<SopInstanceReceivedEventArgs> SopInstanceReceivedEvent;
        public event EventHandler<SeriesCompletedEventArgs> SeriesCompletedEvent;
        public event EventHandler<StudyCompletedEventArgs> StudyCompletedEvent;
        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceivedEvent;

        public DicomClient(ApplicationEntity ownAEParameters)
        {
            // this is a temporary hack to initialize the sockets layer
            // I haven't been able to find how to properly do this
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            _myOwnAE = ownAEParameters;
        }

        public bool Verify(ApplicationEntity ae)
        {
            try
            {
                T_ASC_Network network = new T_ASC_Network(T_ASC_NetworkRole.NET_REQUESTOR, _myOwnAE.Port, _timeout);

                T_ASC_Parameters associationParameters = new T_ASC_Parameters(_defaultPDUSize, _myOwnAE.AE, ae.AE, ae.Host, ae.Port);
                associationParameters.ConfigureForVerification();

                T_ASC_Association association = network.CreateAssociation(associationParameters);
                association.SendCEcho(_cEchoRepeats);
                association.Release();

                return true;
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
               
                return null;
            }
            catch (DicomRuntimeApplicationException e)
            {
                Console.WriteLine("Caught the exception");
                throw e;
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

        private ApplicationEntity _myOwnAE;
        private int _timeout = 500;
        private int _defaultPDUSize = 16384;
        private int _cEchoRepeats = 7;
    }
}
