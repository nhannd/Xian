/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.IO;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public class DicomClient : NetworkBase
    {
        #region Private Members
		private string _host;
		private int _port;
		private string _callingAe;
		private string _calledAe;
		private int _timeout;
		private Socket _socket;
		private Stream _network;
		private ManualResetEvent _closedEvent;
		private bool _closedOnError;
        IDicomClientHandler _handler;
        AssociationParameters _assoc;
		#endregion

		#region Public Constructors
        private DicomClient(String host, int port, AssociationParameters assoc, IDicomClientHandler handler) : base()
        {
            _host = host;
            _port = port;
            _socket = null;
            _network = null;
            _closedEvent = null;
            _timeout = 10;
            _handler = handler;
            _assoc = assoc;
        }
		#endregion

		#region Public Properties
		public string Host {
			get { return _host; }
		}

		public int Port {
			get { return _port; }
		}

		public string CallingAE {
			get { return _callingAe; }
			set { _callingAe = value; }
		}

		public string CalledAE {
			get { return _calledAe; }
			set { _calledAe = value; }
		}

		public int Timeout {
			get { return _timeout; }
			set { _timeout = value; }
		}

		public Socket InternalSocket {
			get { return _socket; }
		}

		//public Logger Log {
	//		get { return _log; }
//			set { _log = value; }
		//}

		public bool ClosedOnError {
			get { return _closedOnError; }
		}
		#endregion

        private void Connect()
        {
            _closedOnError = false;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SendTimeout = _timeout * 1000;
            _socket.ReceiveTimeout = _timeout * 1000;
            _socket.Connect(_host, _port);

            _network = new NetworkStream(_socket);

            InitializeNetwork(_network);

            _closedEvent = new ManualResetEvent(false);

            OnClientConnected();
        }

        private void ConnectTLS()
        {
            _closedOnError = false;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SendTimeout = _timeout * 1000;
            _socket.ReceiveTimeout = _timeout * 1000;
            _socket.Connect(_host, _port);

            _network = new SslStream(new NetworkStream(_socket));

            InitializeNetwork(_network);

            _closedEvent = new ManualResetEvent(false);

            OnClientConnected();
        }

		#region Public Members
        public static DicomClient Connect(string host, int port, ApplicationEntity ae, AssociationParameters assoc, IDicomClientHandler handler)
        {
            DicomClient client = new DicomClient(host,port,assoc,handler);
            client.Connect();
            return client;
		}

        public static DicomClient ConnectTLS(string host, int port, ApplicationEntity ae, AssociationParameters assoc, IDicomClientHandler handler)
        {
            DicomClient client = new DicomClient(host, port, assoc, handler);
            client.ConnectTLS();
            return client;
		}

		public void Close() {
            
            _handler.OnClientClosed(this);

			if (_network != null) {
				_network.Close();
				_network = null;
			}
			if (_socket != null) {
				if (_socket.Connected)
					_socket.Close();
				_socket = null;
			}
			if (_closedEvent != null) {
				_closedEvent.Set();
				_closedEvent = null;
			}
			ShutdownNetwork();
		}

		public bool Wait() {
			_closedEvent.WaitOne();
			return !_closedOnError;
		}
		#endregion

		#region NetworkBase Overrides
        protected void OnClientConnected()
        {
            DicomLogger.LogInfo("SCU -> Connect: {0}", InternalSocket.RemoteEndPoint);


            _assoc.CalledAE = CalledAE;
            _assoc.CallingAE = CallingAE;

            DicomLogger.LogInfo("C-Store SCU -> Association request");
            SendAssociateRequest(_assoc);

        }
		protected override bool NetworkHasData() {
			return _socket.Available > 0;
		}

		protected override void OnNetworkError(Exception e) {
            _handler.OnNetworkError(this,e);
			_closedOnError = true;
			Close();
		}

		protected override void OnDimseTimeout() {
			_closedOnError = true;
			Close();
		}

		protected override void OnReceiveAssociateReject(DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason) {
            
            _handler.OnReceiveAssociateReject(this,result, source, reason);

            _closedOnError = true;
			Close();
		}

		protected override void OnReceiveAbort(DcmAbortSource source, DcmAbortReason reason) {
            
            _handler.OnReceiveAbort(this,source, reason);

			_closedOnError = true;
			Close();
		}

		protected override void OnReceiveReleaseResponse() {

            _handler.OnReceiveReleaseResponse(this);

			_closedOnError = false;
			Close();
		}
        protected override bool OnReceiveDimse(byte pcid, AttributeCollection command, AttributeCollection dataset)
        {
            ushort messageID = command[DicomTags.MessageID].GetUInt16(1);
            DicomPriority priority = (DicomPriority)command[DicomTags.Priority].GetUInt16(0);
            DicomCommandField commandField = (DicomCommandField)command[DicomTags.CommandField].GetUInt16(0);

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
                DicomMessage msg = new DicomMessage(command, dataset);
                _handler.OnReceiveRequestMessage(this, pcid, messageID, msg);
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
                DcmStatus status = DcmStatuses.Lookup(command[DicomTags.Status].GetUInt16(0, 0x0211));
                DicomMessage msg = new DicomMessage(command, dataset);
                _handler.OnReceiveResponseMessage(this, pcid, messageID, status, msg);
                return true;
            }

            return false;
        }
		#endregion
    }
}
