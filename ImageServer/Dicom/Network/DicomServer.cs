/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public delegate IDicomServerHandler StartAssociation(ServerAssociationParameters assoc);

    public class DicomServer : NetworkBase
    {
        #region Static Public Methods
        public static void StartListening(ServerAssociationParameters parameters, StartAssociation acceptor)
        {
            Listener.Listen(parameters, acceptor);
        }

        public static void StopListening(ServerAssociationParameters parameters)
        {
            Listener.StopListening(parameters);
        }
        #endregion

        #region Private Members
        private string _host;
		private Socket _socket;
		private Stream _network;
		private ManualResetEvent _closedEvent;
		private bool _closedOnError = false;
        IDicomServerHandler _handler;
        private Dictionary<String, ListenerInfo> _appList;
		#endregion

        #region Public Properties
        public bool ClosedOnError
        {
            get { return _closedOnError; }
        }
        #endregion

        #region Public Constructors

        internal DicomServer(Socket socket, Dictionary<String,ListenerInfo> appList)
            : base()
        {
            IPEndPoint remote = (IPEndPoint)socket.RemoteEndPoint;

            _host = remote.Address.ToString();

            _socket = socket;
            _network = new NetworkStream(_socket);
            _closedEvent = null;
            _handler = null;
            _appList = appList;

            // Start background thread for incoming associations
            InitializeNetwork(_network, "Server Handler from: " + remote.ToString());
        }
		#endregion

        #region Private Methods
        private void SetSocketOptions(ServerAssociationParameters parameters)
        {
            _socket.ReceiveBufferSize = parameters.ReceiveBufferSize;
            _socket.SendBufferSize = parameters.SendBufferSize;
            _socket.ReceiveTimeout = parameters.ReadTimeout;
            _socket.SendTimeout = parameters.WriteTimeout;
            _socket.LingerState = new LingerOption(false, 5);
            // Nagle option
            _socket.NoDelay = false;
        }

        private bool NegotiateAssociation(AssociationParameters cp, ServerAssociationParameters sp)
        {
            foreach (DicomPresContext clientContext in cp.GetPresentationContexts())
            {
                TransferSyntax selectedSyntax = null;
                foreach (TransferSyntax ts in clientContext.GetTransfers())
                {
                        byte pcid = sp.FindAbstractSyntaxWithTransferSyntax(clientContext.AbstractSyntax, ts);
                        if (pcid != 0)
                        {
                            // TODO Role negotiation here, need to check if roles set, and if so, if they match
                            selectedSyntax = ts;
                            break;
                        
                    }
                }
                if (selectedSyntax != null)
                {
                    clientContext.ClearTransfers();
                    clientContext.AddTransfer(selectedSyntax);
                    clientContext.SetResult(DicomPresContextResult.Accept);
                }
                else
                {
                    // No contexts accepted, set if abstract or transfer syntax reject
                    if (0 == sp.FindAbstractSyntax(clientContext.AbstractSyntax))
                        clientContext.SetResult(DicomPresContextResult.RejectAbstractSyntaxNotSupported);
                    else
                        clientContext.SetResult(DicomPresContextResult.RejectTransferSyntaxesNotSupported);
                }
            }
            bool anyValidContexts = false;

            foreach (DicomPresContext clientContext in cp.GetPresentationContexts())
            {
                if (clientContext.Result == DicomPresContextResult.Accept)
                {
                    anyValidContexts = true;
                    break;
                }
            }
            if (anyValidContexts == false)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Public Methods
        public void Close()
        {
            lock (this)
            {
                if (_network != null)
                {
                    _network.Close();
                    _network = null;
                }
                if (_socket != null)
                {
                    if (_socket.Connected)
                        _socket.Close();
                    _socket = null;
                }
                if (_closedEvent != null)
                {
                    _closedEvent.Set();
                    _closedEvent = null;
                }
            }
            ShutdownNetwork();
        }
        #endregion

        #region NetworkBase Overrides
        protected override bool NetworkHasData()
        {
            return _socket.Available > 0;
        }

        protected override void OnNetworkError(Exception e)
        {
            try
            {
                if (_handler != null)
                    _handler.OnNetworkError(this, this._assoc as ServerAssociationParameters, e);
            }
            catch (Exception) { }

            _closedOnError = true;
            Close();
        }

        protected override void OnReceiveAssociateRequest(ServerAssociationParameters association)
        {
            if (!_appList.ContainsKey(association.CalledAE))
            {
                DicomLogger.LogError("Rejecting association from {0}: Invalid Called AE Title.", association.CallingAE);
                SendAssociateReject(DicomRejectResult.Permanent, DicomRejectSource.ServiceProviderACSE, DicomRejectReason.CalledAENotRecognized);
                return;
            }

            ListenerInfo info = _appList[association.CalledAE];

            // Populate the AssociationParameters properly
            association.ReadTimeout = info.Parameters.ReadTimeout;
            association.ReceiveBufferSize = info.Parameters.ReceiveBufferSize;
            association.WriteTimeout = info.Parameters.WriteTimeout;
            association.SendBufferSize = info.Parameters.SendBufferSize;

            association.RemoteEndPoint = _socket.RemoteEndPoint as IPEndPoint;
            association.LocalEndPoint = _socket.LocalEndPoint as IPEndPoint;


            // Setup Socketoptions based on the user's settings
            SetSocketOptions(association as ServerAssociationParameters);

            // Select the presentation contexts
            bool anyValidContexts = NegotiateAssociation(association, info.Parameters);
            if (!anyValidContexts)
            {
                DicomLogger.LogError("Rejecting association from {0}: No valid presentation contexts.",association.CallingAE);
                SendAssociateReject(DicomRejectResult.Permanent, DicomRejectSource.ServiceProviderACSE, DicomRejectReason.NoReasonGiven);
                return;
            }
            
            _appList = null;

            try
            {
                _handler = info.StartDelegate(association);
                _handler.OnReceiveAssociateRequest(this, association);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveAssociateRequest or StartDelegate");
            }
        }

        protected override void OnDimseTimeout()
        {
            try
            {
                _handler.OnDimseTimeout(this, this._assoc as ServerAssociationParameters);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnDimseTimeout");
            }
        }

        protected override void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            try
            {
                _handler.OnReceiveAbort(this, this._assoc as ServerAssociationParameters, source, reason);
            }
            catch (Exception e) 
            {
                OnUserException(e, "Unexpected exception OnReceiveAbort");
            }

            _closedOnError = true;
            Close();
        }

        protected override void OnReceiveReleaseRequest()
        {
            try
            {
                _handler.OnReceiveReleaseRequest(this, this._assoc as ServerAssociationParameters);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveReleaseRequest");
                return;
            }
            SendReleaseResponse();
        }
        protected override void OnReceiveDimseRequest(byte pcid, DicomMessage msg)
        {
            try
            {
                _handler.OnReceiveRequestMessage(this, this._assoc as ServerAssociationParameters, pcid, msg);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveRequestMessage");
            }
            return ;
        }
        protected override void OnReceiveDimseResponse(byte pcid, DicomMessage msg)
        {

            try
            {
                _handler.OnReceiveResponseMessage(this, this._assoc as ServerAssociationParameters, pcid, msg);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveResponseMessage");
            }
            return;

        }
        #endregion
    }
}
