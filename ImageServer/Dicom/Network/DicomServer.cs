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
    public delegate IDicomServerHandler StartAssociation(AssociationParameters assoc);

    public class DicomServer : NetworkBase
    {
        #region Static Public members
        public static void StartListening(int port, ApplicationEntity ae, StartAssociation acceptor)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            Listener.Listen(ep, ae, acceptor);
        }

        public static void StopListening(int port, ApplicationEntity ae)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            Listener.StopListening(ep, ae);
        }
        #endregion

        #region Private Members
        private string _host;
		private int _port;
		private string _callingAe;
		private string _calledAe;
		private int _timeout;
		private Socket _socket;
		private Stream _network;
		private ManualResetEvent _closedEvent;
		//private Logger _log;
		private bool _closedOnError;
        IDicomServerHandler _handler;
        private Dictionary<String, ListenerInfo> _appList;
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
            _timeout = 10;
            _handler = null;
            _appList = appList;

            // Start background thread for incoming associations
            InitializeNetwork(_network);
        }
		#endregion

        public void Close()
        {
            if (_handler != null)
                _handler.OnClientClosed(this);

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
            ShutdownNetwork();
        }

        #region NetworkBase Overrides
        protected override bool NetworkHasData()
        {
            return _socket.Available > 0;
        }

        protected override void OnNetworkError(Exception e)
        {
            if (_handler != null)
                _handler.OnNetworkError(this,e);
            _closedOnError = true;
            Close();
        }

        protected override void OnReceiveAssociateRequest(AssociationParameters association)
        {
            if (!_appList.ContainsKey(association.CalledAE))
            {
                //TODO check what to do on abort.
                SendAssociateAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);

                _closedOnError = true;
                Close();
            }

            ListenerInfo info = _appList[association.CalledAE];
            _handler = info.StartDelegate(association);
            _handler.OnReceiveAssociateRequest(this, association);
            
            _calledAe = association.CalledAE;
            _callingAe = association.CallingAE;
            _port = info.EndPoint.Port;

            _appList = null;

        }

        protected override void OnDimseTimeout()
        {
            _closedOnError = true;
            Close();
        }

        protected override void OnReceiveAbort(DcmAbortSource source, DcmAbortReason reason)
        {

            _handler.OnReceiveAbort(this,source, reason);

            _closedOnError = true;
            Close();
        }

        protected override void OnReceiveReleaseRequest()
        {

            _handler.OnReceiveReleaseRequest(this);

            _closedOnError = false;
            Close();
        }
        #endregion
    }
}
