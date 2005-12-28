using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.OffisWrapper;

namespace ClearCanvas.Dicom.Network
{
    public delegate void SopInstanceReceiveDelegate(object source, System.EventArgs e);
    public delegate void SeriesCompleteDelegate(object source, System.EventArgs e);
    public delegate void StudyCompleteDelegate(object source, System.EventArgs e);
    public delegate void QueryResultReceiveDelegate(object source, System.EventArgs e);

    public class DicomClient
    {
        public event SopInstanceReceiveDelegate SopInstanceReceiveEvent;
        public event SeriesCompleteDelegate SeriesCompleteEvent;
        public event StudyCompleteDelegate StudyCompleteEvent;
        public event QueryResultReceiveDelegate QueryResultReceiveEvent;

        private ApplicationEntity _myOwnAE;
        private System.Int32 _timeout = 500;
        private System.Int32 _defaultPDUSize = 16384;
        private System.Int32 _cEchoRepeats = 7;

        public DicomClient(ApplicationEntity ownAEParameters)
        {
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
                T_ASC_Association association = network.CreateAssociation(associationParameters);
                association.SendCEcho(_cEchoRepeats);
                
            }
            catch (DicomRuntimeApplicationException e)
            {
                Console.WriteLine("Caught the exception");
            }

            return false;
        }

        public List<QueryResult> Query(ApplicationEntity ae, PatientID patientID, PatientsName patientsName)
        {
            return new List<QueryResult>();
        }

        public List<QueryResult> Query(ApplicationEntity ae, Uid studyInstanceUid)
        {
            if (studyInstanceUid.CompareTo(new Uid("1.2.840.1.2.311432.43242.266")) == 0)
            {
                List<QueryResult> list = new List<QueryResult>();
                list.Add(new QueryResult());
                return list;
            }
            else
            {
                return new List<QueryResult>();
            }
        }

        public void Retrieve(ApplicationEntity ae, Uid uid, System.String path)
        {
            EventsHelper.Fire(SopInstanceReceiveEvent, this, new System.EventArgs());
        }
    }
}
