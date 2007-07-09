using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    internal struct ListenerInfo
    {
        public StartAssociation StartDelegate;
        public ApplicationEntity App;
        public IPEndPoint EndPoint;
    }

    /// <summary>
    /// Class used to create background listen threads for incoming DICOM associations.
    /// </summary>
    internal class Listener : IDisposable
    {
        #region Members

        static private Dictionary<IPEndPoint, Listener> _listeners = new Dictionary<IPEndPoint, Listener>();
        private static IPEndPoint _ipEndPoint = null;
        private Dictionary<String, ListenerInfo> _applications = new Dictionary<String, ListenerInfo>();
        private TcpListener _tcpListener = null;
        private Thread _theThread = null;
        private bool _stop = false;
        public static ManualResetEvent _threadStop = new ManualResetEvent(false);

        #endregion

        #region Public Static Methods

        public static void Listen(IPEndPoint ep, ApplicationEntity ae, StartAssociation acceptor)
        {
            if (_listeners.ContainsKey(ep))
            {
                Listener theListener = _listeners[ep];

                ListenerInfo info = new ListenerInfo();
                
                info.StartDelegate = acceptor;
                info.App = ae;
                info.EndPoint = ep;

                theListener._applications.Add(ae.Name,info);

            }
            else
            {
                
                Listener theListener = new Listener(ep, ae, acceptor);
                _listeners[ep] = theListener;
                theListener.StartThread();
            }
        }

        public static void StopListening(IPEndPoint ep, ApplicationEntity ae)
        {
            Listener theListener;

            theListener = _listeners[ep];
            if (theListener != null)
            {
                if (theListener._applications.ContainsKey(ae.Name))
                {
                    theListener._applications.Remove(ae.Name);

                    if (theListener._applications.Count == 0)
                    {
                        // Cleanup the listener
                        _listeners[ep] = null;
                        theListener.StopThread();
                        theListener.Dispose();
                    }
                }
            }

        }
        #endregion

        #region Constructors
        internal Listener(IPEndPoint ep, ApplicationEntity ae, StartAssociation acceptor)
        {

            ListenerInfo info = new ListenerInfo();
            info.App = ae;
            info.StartDelegate = acceptor;
            info.EndPoint = ep;

            this._applications.Add(ae.Name, info);

            _ipEndPoint = ep;
        }
        #endregion

        public void StartThread()
        {
            
            _theThread = new Thread(new ThreadStart(Listen));
            _theThread.Name = "Association Listener on port " + _ipEndPoint.Port;

            _theThread.Start();
        }

        public void StopThread()
        {
            _stop = true;
            _threadStop.Set();

            _theThread.Join();
        }

        public void Listen()
        {
            _tcpListener = new TcpListener(_ipEndPoint);

            try
            {
                _tcpListener.Start();
            }
            catch (SocketException e)
            {
                DicomLogger.LogError("Unexpected exception when starting TCP listener: " + e.ToString());
                return;
            }

            while (true)
            {
                _tcpListener.BeginAcceptSocket(
                    new AsyncCallback(DoBeginAcceptCallback), this);

                _threadStop.WaitOne();

                if (_stop == true)
                    return;

                // We have a connection
                
            }
        }

        private void DoBeginAcceptCallback(IAsyncResult ar)
        {
            if (_tcpListener == null || _stop == true)
                return;

            Socket theClient = _tcpListener.EndAcceptSocket(ar);
            DicomServer server = new DicomServer(theClient,_applications);

            _threadStop.Set();
        }

        #region IDisposable Implementation
        public void Dispose()
        {
            StopThread();
            if (_tcpListener != null)
            {
                _tcpListener.Stop();
                _tcpListener = null;
            }
        }
        #endregion
    }
}
