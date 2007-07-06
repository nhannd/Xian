/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom.Network
{


    public enum DicomQueryRetrieveLevel
    {
        Patient,
        Study,
        Series,
        Instance,
        Worklist
    }

    public enum DicomPriority : ushort
    {
        Low = 0x0002,
        Medium = 0x0000,
        High = 0x0001
    }

    public enum DicomCommandField : ushort
    {
        CStoreRequest = 0x0001,
        CStoreResponse = 0x8001,
        CGetRequest = 0x0010,
        CGetResponse = 0x8010,
        CFindRequest = 0x0020,
        CFindResponse = 0x8020,
        CMoveRequest = 0x0021,
        CMoveResponse = 0x8021,
        CEchoRequest = 0x0030,
        CEchoResponse = 0x8030,
        NEventReportRequest = 0x0100,
        NEventReportResponse = 0x8100,
        NGetRequest = 0x0110,
        NGetResponse = 0x8110,
        NSetRequest = 0x0120,
        NSetResponse = 0x8120,
        NActionRequest = 0x0130,
        NActionResponse = 0x8130,
        NCreateRequest = 0x0140,
        NCreateResponse = 0x8140,
        NDeleteRequest = 0x0150,
        NDeleteResponse = 0x8150,
        CCancelRequest = 0x0FFF
    }

    internal class DcmDimseInfo
    {
        public AttributeCollection Command;
        public AttributeCollection Dataset;
        public ChunkStream CommandData;
        public ChunkStream DatasetData;
        public DicomStreamReader CommandReader;
        public DicomStreamReader DatasetReader;
        public TransferMonitor Stats;
        public bool IsNewDimse;

        public DcmDimseInfo()
        {
            Stats = new TransferMonitor();
            IsNewDimse = true;
        }
    }

    public abstract class NetworkBase
    {
        #region Protected Members
        private ushort _messageId;
        private Stream _network;
        private AssociationParameters _assoc;
        private DcmDimseInfo _dimse;
        private Thread _thread;
        private bool _stop;
        private int _dimseTimeout;
        #endregion

        #region Public Constructors
        public NetworkBase()
        {
            _messageId = 1;
            _dimseTimeout = 180;
        }

        #endregion

        #region Public Properties
        public AssociationParameters Associate
        {
            get { return _assoc; }
        }

        public int DimseTimeout
        {
            get { return _dimseTimeout; }
            set { _dimseTimeout = value; }
        }

        protected Stream InternalStream
        {
            get { return _network; }
        }
        #endregion

        #region Protected Methods
        protected void InitializeNetwork(Stream network)
        {
            _network = network;
            _stop = false;
            _thread = new Thread(new ThreadStart(Process));
            _thread.Start();
        }

        protected void ShutdownNetwork()
        {
            _stop = true;
            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
        }

        protected abstract bool NetworkHasData();

        protected virtual void OnNetworkError(Exception e)
        {
            
        }

        protected virtual void OnDimseTimeout()
        {
        }

        protected virtual void OnReceiveAbort(DcmAbortSource source, DcmAbortReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveAssociateRequest(AssociationParameters association)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveAssociateAccept(AssociationParameters association)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveReleaseRequest()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveReleaseResponse()
        {
            throw new Exception("The method or operation is not implemented.");
        }



        protected virtual void OnReceiveDimseBegin(byte pcid, AttributeCollection command, AttributeCollection dataset, TransferMonitor stats)
        {
        }

        protected virtual void OnReceiveDimseProgress(byte pcid, AttributeCollection command, AttributeCollection dataset, TransferMonitor stats)
        {
        }

        protected virtual bool OnReceiveDimse(byte pcid, AttributeCollection command, AttributeCollection dataset)
        {
            return false;
        }

        protected virtual void OnSendDimseBegin(byte pcid, AttributeCollection command, AttributeCollection dataset, TransferMonitor monitor)
        {
        }

        protected virtual void OnSendDimseProgress(byte pcid, AttributeCollection command, AttributeCollection dataset, TransferMonitor monitor)
        {
        }

        protected virtual void OnSendDimse(byte pcid, AttributeCollection command, AttributeCollection dataset)
        {
        }
        #endregion

        #region Public Methods
        protected ushort NextMessageID()
        {
            return _messageId++;
        }

        public void SendAssociateRequest(AssociationParameters associate)
        {
            _assoc = associate;
            AAssociateRQ pdu = new AAssociateRQ(_assoc);
            SendRawPDU(pdu.Write());
        }

        public void SendAssociateAbort(DcmAbortSource source, DcmAbortReason reason)
        {
            AAbort pdu = new AAbort(source,reason);
            SendRawPDU(pdu.Write());
        }

        public void SendAssociateAccept(AssociationParameters associate)
        {
            AAssociateAC pdu = new AAssociateAC(_assoc);
            SendRawPDU(pdu.Write());
        }

        public void SendAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason)
        {
            AAssociateRJ pdu = new AAssociateRJ(result, source, reason);
            SendRawPDU(pdu.Write());
        }

        public void SendReleaseRequest()
        {
            AReleaseRQ pdu = new AReleaseRQ();
            SendRawPDU(pdu.Write());
        }

        public void SendReleaseResponse()
        {
            AReleaseRP pdu = new AReleaseRP();
            SendRawPDU(pdu.Write());
        }

        public void SendCEchoRequest(byte presentationID, ushort messageID)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);
            AttributeCollection command = CreateRequest(messageID, DicomCommandField.CEchoRequest, affectedClass, false);
            SendDimse(presentationID, command, null);
        }

        public void SendCEchoResponse(byte presentationID, ushort messageID, DcmStatus status)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);
            AttributeCollection command = CreateResponse(messageID, DicomCommandField.CEchoResponse, affectedClass, status);
            SendDimse(presentationID, command, null);
        }

        public void SendCStoreRequest(byte presentationID, ushort messageID, DicomUid affectedInstance,
            DicomPriority priority, AttributeCollection dataset)
        {
            SendCStoreRequest(presentationID, messageID, affectedInstance, priority, null, 0, dataset);
        }

        public void SendCStoreRequest(byte presentationID, ushort messageID, DicomUid affectedInstance,
            DicomPriority priority, string moveAE, ushort moveMessageID, AttributeCollection dataset)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);

            AttributeCollection command = CreateRequest(messageID, DicomCommandField.CStoreRequest, affectedClass, true);
            command[DicomTags.Priority].Values = (ushort)priority;
            command[DicomTags.AffectedSOPInstanceUID].Values = affectedInstance.UID;
            if (moveAE != null && moveAE != String.Empty)
            {
                command[DicomTags.MoveOriginatorApplicationEntityTitle].Values = moveAE;
                command[DicomTags.MoveOriginatorMessageID].Values = moveMessageID;
            }

            SendDimse(presentationID, command, dataset);
        }

        public void SendCStoreResponse(byte presentationID, ushort messageID, DicomUid affectedInstance, DcmStatus status)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);
            AttributeCollection command = CreateResponse(messageID, DicomCommandField.CStoreResponse, affectedClass, status);
            command[DicomTags.AffectedSOPInstanceUID].Values = affectedInstance.UID;
            SendDimse(presentationID, command, null);
        }

        public void SendCFindRequest(byte presentationID, ushort messageID, AttributeCollection dataset)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);
            AttributeCollection command = CreateRequest(messageID, DicomCommandField.CFindRequest, affectedClass, true);
            SendDimse(presentationID, command, dataset);
        }

        public void SendCMoveRequest(byte presentationID, ushort messageID, string destinationAE, AttributeCollection dataset)
        {
            DicomUid affectedClass = Associate.GetAbstractSyntax(presentationID);
            AttributeCollection command = CreateRequest(messageID, DicomCommandField.CMoveRequest, affectedClass, true);
            command[DicomTags.MoveDestination].Values = destinationAE;
            SendDimse(presentationID, command, dataset);
        }
        #endregion

        #region Private Methods
        private AttributeCollection CreateRequest(ushort messageID, DicomCommandField commandField, DicomUid affectedClass, bool hasDataset)
        {
            AttributeCollection command = new AttributeCollection();
            command[DicomTags.MessageID].Values = messageID;
            command[DicomTags.CommandField].Values = (ushort)commandField;
            command[DicomTags.AffectedSOPClassUID].Values = affectedClass.UID;
            command[DicomTags.DataSetType].Values = hasDataset ? (ushort)0x0202 : (ushort)0x0101;
            return command;
        }

        private AttributeCollection CreateResponse(ushort messageIdRespondedTo, DicomCommandField commandField, DicomUid affectedClass, DcmStatus status)
        {
            AttributeCollection command = new AttributeCollection();
            command[DicomTags.MessageIDBeingRespondedTo].Values = messageIdRespondedTo;
            command[DicomTags.CommandField].Values = (ushort)commandField;
            command[DicomTags.AffectedSOPClassUID].Values = affectedClass.UID;
            command[DicomTags.DataSetType].Values = (ushort)0x0101;
            command[DicomTags.Status].Values = status.Code;
            return command;
        }

        private void Process()
        {
            try
            {
                DateTime timeout = DateTime.Now.AddSeconds(DimseTimeout);
                while (!_stop)
                {
                    if (NetworkHasData())
                    {
                        timeout = DateTime.Now.AddSeconds(DimseTimeout);
                        ProcessNextPDU();
                    }
                    else if (DateTime.Now > timeout)
                    {
                        OnDimseTimeout();
                        _stop = true;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                _network.Close();
                _network = null;
            }
            catch (Exception e)
            {
                OnNetworkError(e);
            }
        }

        private bool ProcessNextPDU()
        {
            RawPDU raw = new RawPDU(_network);

            if (raw.Type == 0x04)
            {
                if (_dimse == null)
                {
                    _dimse = new DcmDimseInfo();
                }
            }

            raw.ReadPDU();

            try
            {
                switch (raw.Type)
                {
                    case 0x01:
                        {
                            _assoc = new AssociationParameters();
                            AAssociateRQ pdu = new AAssociateRQ(_assoc);
                            pdu.Read(raw);
                            OnReceiveAssociateRequest(_assoc);
                            return true;
                        }
                    case 0x02:
                        {
                            AAssociateAC pdu = new AAssociateAC(_assoc);
                            pdu.Read(raw);
                            OnReceiveAssociateAccept(_assoc);
                            return true;
                        }
                    case 0x03:
                        {
                            AAssociateRJ pdu = new AAssociateRJ();
                            pdu.Read(raw);
                            OnReceiveAssociateReject(pdu.Result, pdu.Source, pdu.Reason);
                            return true;
                        }
                    case 0x04:
                        {
                            PDataTF pdu = new PDataTF();
                            pdu.Read(raw);
                            return ProcessPDataTF(pdu);
                        }
                    case 0x05:
                        {
                            AReleaseRQ pdu = new AReleaseRQ();
                            pdu.Read(raw);
                            OnReceiveReleaseRequest();
                            return true;
                        }
                    case 0x06:
                        {
                            AReleaseRP pdu = new AReleaseRP();
                            pdu.Read(raw);
                            OnReceiveReleaseResponse();
                            return true;
                        }
                    case 0x07:
                        {
                            AAbort pdu = new AAbort();
                            pdu.Read(raw);
                            OnReceiveAbort(pdu.Source, pdu.Reason);
                            return true;
                        }
                    case 0xFF:
                        {
                            return false;
                        }
                    default:
                        throw new NetworkException("Unknown PDU type");
                }
            }
            catch (Exception e)
            {
                OnNetworkError(e);
                String file = String.Format(@"{0}\Errors\{1}.pdu",
                    Environment.CurrentDirectory, DateTime.Now.Ticks);
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Errors");
                raw.Save(file);
                return false;
            }
        }

        private bool ProcessPDataTF(PDataTF pdu)
        {
            try
            {
                int bytes = 0, total = 0;
                byte pcid = 0;
                foreach (PDV pdv in pdu.PDVs)
                {
                    pcid = pdv.PCID;
                    if (pdv.IsCommand)
                    {
                        if (_dimse.CommandData == null)
                            _dimse.CommandData = new ChunkStream();

                        _dimse.CommandData.AddChunk(pdv.Value);

                        if (_dimse.Command == null)
                        {
                            _dimse.Command = new AttributeCollection();
                        }

                        if (_dimse.CommandReader == null)
                        {
                            _dimse.CommandReader = new DicomStreamReader(_dimse.CommandData);
                            _dimse.CommandReader.Dataset = _dimse.Command;
                        }

                        _dimse.CommandReader.Read(null, DicomReadOptions.Default);

                        bytes += pdv.Value.Length;
                        total = (int)_dimse.CommandReader.BytesEstimated;

                        if (pdv.IsLastFragment)
                        {
                            _dimse.CommandData = null;
                            _dimse.CommandReader = null;

                            bool isLast = true;
                            if (_dimse.Command.Contains(DicomTags.DataSetType))
                            {
                                if (_dimse.Command[DicomTags.DataSetType].GetUInt16(0) != 0x0101)
                                    isLast = false;
                            }
                            if (isLast)
                            {
                                _dimse.Stats.Tick(bytes, total);
                                if (_dimse.IsNewDimse)
                                    OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                                OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                                bool ret = OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset);
                                _dimse = null;
                                return ret;
                            }
                        }
                    }
                    else
                    {
                        if (_dimse.DatasetData == null)
                            _dimse.DatasetData = new ChunkStream();

                        _dimse.DatasetData.AddChunk(pdv.Value);

                        if (_dimse.Dataset == null)
                        {
                            TransferSyntax ts = _assoc.GetAcceptedTransferSyntax(pdv.PCID);
                            _dimse.Dataset = new AttributeCollection();
                        }

                        if (_dimse.DatasetReader == null)
                        {
                            _dimse.DatasetReader = new DicomStreamReader(_dimse.DatasetData);
                            _dimse.DatasetReader.Dataset = _dimse.Dataset;
                        }

                        _dimse.DatasetReader.Read(null, DicomReadOptions.Default);

                        bytes += pdv.Value.Length;
                        total = (int)_dimse.DatasetReader.BytesEstimated;

                        if (pdv.IsLastFragment)
                        {
                            _dimse.CommandData = null;
                            _dimse.CommandReader = null;

                            _dimse.Stats.Tick(bytes, total);
                            if (_dimse.IsNewDimse)
                                OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                            OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                            bool ret = OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset);
                            _dimse = null;
                            return ret;
                        }
                    }
                }

                _dimse.Stats.Tick(bytes, total);

                if (_dimse.IsNewDimse)
                {
                    OnReceiveDimseBegin(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                    _dimse.IsNewDimse = false;
                }
                else
                {
                    OnReceiveDimseProgress(pcid, _dimse.Command, _dimse.Dataset, _dimse.Stats);
                }

                return true;
            }
            catch (Exception e)
            {
                //do something here!
                DicomLogger.LogError(e.ToString());
                return false;
            }
        }

        private void SendRawPDU(RawPDU pdu)
        {
            try
            {
                pdu.WritePDU(_network);
            }
            catch (Exception e)
            {
                OnNetworkError(e);
            }
        }

        private bool SendDimse(byte pcid, AttributeCollection command, AttributeCollection dataset)
        {
            try
            {
                TransferSyntax ts = _assoc.GetAcceptedTransferSyntax(pcid);

                int total = (int)command.CalculateWriteLength(TransferSyntax.GetTransferSyntax(TransferSyntax.ImplicitVRLittleEndian), DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

                if (dataset != null)
                    total += (int)dataset.CalculateWriteLength(ts, DicomWriteOptions.Default);

                PDataTFStream pdustream = new PDataTFStream(_network, pcid, (int)_assoc.MaximumPduLength, total);
                pdustream.OnTick += delegate(TransferMonitor stats)
                {
                    OnSendDimseProgress(pcid, command, dataset, stats);
                };

                OnSendDimseBegin(pcid, command, dataset, pdustream.Stats);

                DicomStreamWriter dsw = new DicomStreamWriter(pdustream);
                dsw.Write(TransferSyntax.GetTransferSyntax(TransferSyntax.ImplicitVRLittleEndian),
                    command, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

                if (dataset != null)
                {
                    pdustream.IsCommand = false;
                    dsw.Write(ts, dataset, DicomWriteOptions.Default);
                }

                // flush last pdu
                pdustream.Flush(true);

                OnSendDimse(pcid, command, dataset);

                return true;
            }
            catch (Exception e)
            {
                OnNetworkError(e);
                return false;
            }
        }
        #endregion
    }
}
 
