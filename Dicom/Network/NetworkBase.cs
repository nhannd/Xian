using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Network
{
    internal enum DicomAssociationState
    {
        Sta1_Idle,
        Sta2_TransportConnectionOpen,
        Sta3_AwaitingLocalAAssociationResponsePrimative,
        Sta4_AwaitingTransportConnectionOpeningToComplete,
        Sta5_AwaitingAAssociationACOrReject,
        Sta6_AssociationEstablished,
        Sta7_AwaitingAReleaseRP,
        Sta8_AwaitingAReleaseRPLocalUser,
        Sta9_ReleaseCollisionRequestorSide,
        Sta10_ReleaseCollisionAcceptorSide,
        Sta11_ReleaseCollisionRequestorSide,
        Sta12_ReleaseCollisionAcceptorSide,
        Sta13_AwaitingTransportConnectionClose
    }

    public enum DicomQueryRetrieveLevel
    {
        Patient,
        Study,
        Series,
        Instance,
        Worklist
    }

    /// <summary>
    /// An enumerated value representing the priority values encoded in the tag <see cref="DicomTags.Priority"/>.
    /// </summary>
    public enum DicomPriority : ushort
    {
        Low = 0x0002,
        Medium = 0x0000,
        High = 0x0001
    }

    /// <summary>
    /// An enumerated value represneting the values for the tag <see cref="DicomTags.CommandField"/>.
    /// </summary>
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
        public DicomAttributeCollection Command;
        public DicomAttributeCollection Dataset;
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

    /// <summary>
    /// Class used for DICOM network communications.
    /// </summary>
    /// <remarks>
    /// The classes <see cref="DicomClient"/>"/> and <see cref="DicomServer"/> inherit from this class, to implement network functionality.
    /// </remarks>
    public abstract class NetworkBase
    {
        #region Protected Members
        private ushort _messageId;
        private Stream _network;
        protected AssociationParameters _assoc;
        private DcmDimseInfo _dimse;
        private Thread _thread;
        private bool _stop;
        private int _dimseTimeout;
        private DicomAssociationState _state = DicomAssociationState.Sta1_Idle;
        #endregion

        #region Public Constructors
        public NetworkBase()
        {
            _messageId = 1;
            _dimseTimeout = 180;
        }

        #endregion

        #region Public Properties
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
        protected void InitializeNetwork(Stream network, String name)
        {
            _network = network;
            _stop = false;
            _thread = new Thread(new ThreadStart(Process));
            _thread.Name = name;
            
            _thread.Start();
        }

        protected void ShutdownNetwork()
        {
            _stop = true;
            if (_thread != null)
            {
                if (!Thread.CurrentThread.Equals(_thread))
                {
                    _thread.Join();
                    _thread = null;
                }
            }
        }

        protected abstract bool NetworkHasData();

        protected virtual void OnUserException(Exception e, String description)
        {
            DicomLogger.LogErrorException(e,"Unexpected User exception, description: " + description);
            switch (_state)
            {
                case DicomAssociationState.Sta2_TransportConnectionOpen:
                    break;
                case DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative:
                    break;
                case DicomAssociationState.Sta4_AwaitingTransportConnectionOpeningToComplete:
                    break;
                case DicomAssociationState.Sta5_AwaitingAAssociationACOrReject:
                    break;
                case DicomAssociationState.Sta6_AssociationEstablished:
                    SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
                    break;
                case DicomAssociationState.Sta7_AwaitingAReleaseRP:
                    break;
                case DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Callback called on a network error.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNetworkError(Exception e)
        {            
        }

        /// <summary>
        /// Callback called on a timeout.
        /// </summary>
        protected virtual void OnDimseTimeout()
        {
        }

        protected virtual void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveAssociateRequest(ServerAssociationParameters association)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void OnReceiveAssociateAccept(AssociationParameters association)
        {
        }

        protected virtual void OnReceiveAssociateReject(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
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


        protected virtual void OnReceiveDimseBegin(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, TransferMonitor stats)
        {
        }

        protected virtual void OnReceiveDimseProgress(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, TransferMonitor stats)
        {
        }

        protected virtual void OnReceiveDimseRequest(byte pcid, DicomMessage msg)
        {
        }

        protected virtual void OnReceiveDimseResponse(byte pcid, DicomMessage msg)
        {
        }

        private bool OnReceiveDimse(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
        {
            DicomMessage msg = new DicomMessage(command, dataset);
            DicomCommandField commandField = msg.CommandField;

            if ((commandField == DicomCommandField.CStoreRequest)
                || (commandField == DicomCommandField.CCancelRequest)
                || (commandField == DicomCommandField.CEchoRequest)
                || (commandField == DicomCommandField.CFindRequest)
                || (commandField == DicomCommandField.CGetRequest)
                || (commandField == DicomCommandField.CMoveRequest)
                || (commandField == DicomCommandField.NActionRequest)
                || (commandField == DicomCommandField.NCreateRequest)
                || (commandField == DicomCommandField.NDeleteRequest)
                || (commandField == DicomCommandField.NEventReportRequest)
                || (commandField == DicomCommandField.NGetRequest)
                || (commandField == DicomCommandField.NSetRequest))
            {
                OnReceiveDimseRequest(pcid, msg);
     
                return true;
            }

            if ((commandField == DicomCommandField.CStoreResponse)
             || (commandField == DicomCommandField.CEchoResponse)
             || (commandField == DicomCommandField.CFindResponse)
             || (commandField == DicomCommandField.CGetResponse)
             || (commandField == DicomCommandField.CMoveResponse)
             || (commandField == DicomCommandField.NActionResponse)
             || (commandField == DicomCommandField.NCreateResponse)
             || (commandField == DicomCommandField.NDeleteResponse)
             || (commandField == DicomCommandField.NEventReportResponse)
             || (commandField == DicomCommandField.NGetResponse)
             || (commandField == DicomCommandField.NSetResponse))
            {
                OnReceiveDimseResponse(pcid, msg);

                return true;
            }
            return false;
        }

        protected virtual void OnSendDimseBegin(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, TransferMonitor monitor)
        {
        }

        protected virtual void OnSendDimseProgress(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, TransferMonitor monitor)
        {
        }

        protected virtual void OnSendDimse(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
        {
        }
        #endregion

        #region Public Methods
        public ushort NextMessageID()
        {
            return _messageId++;
        }

        public void SendAssociateRequest(AssociationParameters associate)
        {
            _assoc = associate;
            AAssociateRQ pdu = new AAssociateRQ(_assoc);
            SendRawPDU(pdu.Write());
        }

        public void SendAssociateAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            if (_state != DicomAssociationState.Sta13_AwaitingTransportConnectionClose)
            {
                AAbort pdu = new AAbort(source, reason);
                SendRawPDU(pdu.Write());
                _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
            }
        }

        public void SendAssociateAccept(AssociationParameters associate)
        {
            if (_state != DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative)
            {
                DicomLogger.LogError("Error attempting to send association accept at invalid time in association.");
                SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
                throw new NetworkException("Attempting to send association accept at invalid time in association, aborting");
            }
            AAssociateAC pdu = new AAssociateAC(_assoc);
            SendRawPDU(pdu.Write());

            _state = DicomAssociationState.Sta6_AssociationEstablished;
        }

        public void SendAssociateReject(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {
            if (_state != DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative)
            {
                DicomLogger.LogError("Error attempting to send associaiton reject at invalid time in association.");
                SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
                throw new NetworkException("Attempting to send association reject at invalid time in association, aborting");
            }
            AAssociateRJ pdu = new AAssociateRJ(result, source, reason);
            SendRawPDU(pdu.Write());

            _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
        }

        public void SendReleaseRequest()
        {
            if (_state != DicomAssociationState.Sta6_AssociationEstablished)
            {
                DicomLogger.LogError("Unexpected attempt to send Release Request when in invalid state.");
                return;
            }

            AReleaseRQ pdu = new AReleaseRQ();
            SendRawPDU(pdu.Write());

            _state = DicomAssociationState.Sta7_AwaitingAReleaseRP;
        }

        protected void SendReleaseResponse()
        {
            if (_state != DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser)
            {
            }

            AReleaseRP pdu = new AReleaseRP();
            SendRawPDU(pdu.Write());
            _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
        }

        public void SendCEchoRequest(byte presentationID, ushort messageID)
        {
            DicomLogger.LogInfo("Sending C Echo request, pres ID: {0}, messageID = {1}", presentationID, messageID);

            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
            DicomAttributeCollection command = CreateRequest(messageID, DicomCommandField.CEchoRequest, affectedClass, false);
            SendDimse(presentationID, command, null);
        }

        public void SendCEchoResponse(byte presentationID, ushort messageID, DicomStatus status)
        {
            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
            DicomMessage msg = CreateResponse(messageID, DicomCommandField.CEchoResponse, affectedClass, status);
            SendDimse(presentationID, msg.CommandSet, null);
        }

        public void SendCStoreRequest(byte presentationID, ushort messageID,
            DicomPriority priority, DicomMessage message)
        {
            SendCStoreRequest(presentationID, messageID, priority, null, 0, message);
        }

        public void SendCStoreRequest(byte presentationID, ushort messageID,
            DicomPriority priority, string moveAE, ushort moveMessageID, DicomMessage message)
        {
            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);

            DicomAttributeCollection command = message.MetaInfo;

            message.MessageId = messageID;
            message.CommandField = DicomCommandField.CStoreRequest;
            message.AffectedSopClassUid = message.SopClass.Uid;
            //message.DataSetType = true ? (ushort)0x0202 : (ushort)0x0101;
            message.DataSetType = (ushort)0x0202;
            message.Priority = priority;

            String sopInstanceUid;
            bool ok = message.DataSet[DicomTags.SOPInstanceUID].TryGetString(0, out sopInstanceUid);
            if (!ok)
                throw new DicomException("SOP Instance UID unexpectedly not set in CStore Message being sent.");

            message.AffectedSopInstanceUid = sopInstanceUid;
            
            
            if (moveAE != null && moveAE != String.Empty)
            {
                message.MoveOriginatorApplicationEntityTitle = moveAE;
                message.MoveOriginatorMessageId = moveMessageID;
            }

            SendDimse(presentationID, command, message.DataSet);
        }

        public void SendCStoreResponse(byte presentationID, ushort messageID, DicomUid affectedInstance, DicomStatus status)
        {
            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
            DicomMessage msg = CreateResponse(messageID, DicomCommandField.CStoreResponse, affectedClass, status);
            msg.AffectedSopInstanceUid = affectedInstance.UID;
            SendDimse(presentationID, msg.CommandSet, null);
        }

        public void SendCFindRequest(byte presentationID, ushort messageID, DicomAttributeCollection dataset)
        {
            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
            DicomAttributeCollection command = CreateRequest(messageID, DicomCommandField.CFindRequest, affectedClass, true);
            SendDimse(presentationID, command, dataset);
        }

        public void SendCMoveRequest(byte presentationID, ushort messageID, string destinationAE, DicomAttributeCollection dataset)
        {
            DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
            DicomAttributeCollection command = CreateRequest(messageID, DicomCommandField.CMoveRequest, affectedClass, true);
            command[DicomTags.MoveDestination].Values = destinationAE;
            SendDimse(presentationID, command, dataset);
        }
        #endregion

        #region Private Methods
        private DicomAttributeCollection CreateRequest(ushort messageID, DicomCommandField commandField, DicomUid affectedClass, bool hasDataset)
        {
            DicomAttributeCollection command = new DicomAttributeCollection(0x00000000,0x0000FFFF);
            command[DicomTags.MessageID].Values = messageID;
            command[DicomTags.CommandField].Values = (ushort)commandField;
            command[DicomTags.AffectedSOPClassUID].Values = affectedClass.UID;
            command[DicomTags.DataSetType].Values = hasDataset ? (ushort)0x0202 : (ushort)0x0101;
            return command;
        }

        private DicomMessage CreateResponse(ushort messageIdRespondedTo, DicomCommandField commandField, DicomUid affectedClass, DicomStatus status)
        {
            DicomMessage msg = new DicomMessage();
            msg.MessageIdBeingRespondedTo = messageIdRespondedTo;
            msg.CommandField = commandField;
            msg.AffectedSopClassUid = affectedClass.UID;
            msg.DataSetType = (ushort)0x0101;
            msg.Status = status;
            return msg;
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
                        bool success = ProcessNextPDU();
                        if (!success)
                        {
                            // TODO
                        }
                    }
                    else if (DateTime.Now > timeout)
                    {
                        if (_state == DicomAssociationState.Sta6_AssociationEstablished)
                        {
                            OnDimseTimeout();
                            timeout = DateTime.Now.AddSeconds(DimseTimeout);
                        } 
                        else if (_state == DicomAssociationState.Sta2_TransportConnectionOpen)
                        {
                            DicomLogger.LogError("ARTIM timeout when waiting for AAssociate Request PDU, closing connection.");
                            _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
                            _network.Close(); // TODO
                            
                        }
                        else
                        {
                            OnDimseTimeout();
                            timeout = DateTime.Now.AddSeconds(DimseTimeout);
                        }
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
                            _assoc = new ServerAssociationParameters();
                            AAssociateRQ pdu = new AAssociateRQ(_assoc);
                            pdu.Read(raw);
                            _state = DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative;
                            OnReceiveAssociateRequest(_assoc as ServerAssociationParameters);

                            if (_state != DicomAssociationState.Sta13_AwaitingTransportConnectionClose &&
                                _state != DicomAssociationState.Sta6_AssociationEstablished)
                            {
                                DicomLogger.LogError("Association incorrectly not accepted or rejected, aborting.");
                                return false;
                            }
                            return true;
                        }
                    case 0x02:
                        {
                            AAssociateAC pdu = new AAssociateAC(_assoc);
                            pdu.Read(raw);
                            _state = DicomAssociationState.Sta6_AssociationEstablished;
                            OnReceiveAssociateAccept(_assoc);
                            return true;
                        }
                    case 0x03:
                        {
                            AAssociateRJ pdu = new AAssociateRJ();
                            pdu.Read(raw);
                            _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
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
                            _state = DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser;
                            OnReceiveReleaseRequest();
                            return true;
                        }
                    case 0x06:
                        {
                            AReleaseRP pdu = new AReleaseRP();
                            pdu.Read(raw);
                            _state = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
                            OnReceiveReleaseResponse();
                            return true;
                        }
                    case 0x07:
                        {
                            AAbort pdu = new AAbort();
                            pdu.Read(raw);
                            _state = DicomAssociationState.Sta1_Idle;
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
                            _dimse.Command = new DicomAttributeCollection(0x00000000, 0x0000FFFF);
                        }

                        if (_dimse.CommandReader == null)
                        {
                            _dimse.CommandReader = new DicomStreamReader(_dimse.CommandData);
                            _dimse.CommandReader.TransferSyntax = TransferSyntax.ImplicitVRLittleEndian;
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
                                if (_dimse.Command[DicomTags.DataSetType].GetUInt16(0,0x0) != 0x0101)
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
                            
                            _dimse.Dataset = new DicomAttributeCollection(0x00080000,0xFFFFFFFF);
                        }

                        if (_dimse.DatasetReader == null)
                        {
                            _dimse.DatasetReader = new DicomStreamReader(_dimse.DatasetData);
                            _dimse.DatasetReader.TransferSyntax = _assoc.GetAcceptedTransferSyntax(pdv.PCID);
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
                DicomLogger.LogErrorException(e,"Unexpected exception processing P-DATA PDU");
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

        private bool SendDimse(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
        {
            try
            {
                TransferSyntax ts = _assoc.GetAcceptedTransferSyntax(pcid);

                int total = (int)command.CalculateWriteLength(TransferSyntax.ImplicitVRLittleEndian, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

                if (dataset != null)
                    total += (int)dataset.CalculateWriteLength(ts, DicomWriteOptions.Default);

                PDataTFStream pdustream = new PDataTFStream(_network, pcid, (int)_assoc.MaximumPduLength, total);
                pdustream.OnTick += delegate(TransferMonitor stats)
                {
                    OnSendDimseProgress(pcid, command, dataset, stats);
                };

                OnSendDimseBegin(pcid, command, dataset, pdustream.Stats);

                DicomStreamWriter dsw = new DicomStreamWriter(pdustream);
                dsw.Write(TransferSyntax.ImplicitVRLittleEndian,
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
 
