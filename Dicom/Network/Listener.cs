using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Network
{
    internal struct ListenerInfo
    {
        public StartAssociation StartDelegate;
        public ServerAssociationParameters Parameters;
    }

    /// <summary>
    /// Class used to create background listen threads for incoming DICOM associations.
    /// </summary>
    internal class Listener : IDisposable
    {
        #region Members

        static private Dictionary<IPEndPoint, Listener> _listeners = new Dictionary<IPEndPoint, Listener>();
        private IPEndPoint _ipEndPoint = null;
        private Dictionary<String, ListenerInfo> _applications = new Dictionary<String, ListenerInfo>();
        private TcpListener _tcpListener = null;
        private Thread _theThread = null;
        private bool _stop = false;

        #endregion

        #region Public Static Methods

        public static bool Listen(ServerAssociationParameters parameters, StartAssociation acceptor)
        {
            if (_listeners.ContainsKey(parameters.LocalEndPoint))
            {
                Listener theListener = _listeners[parameters.LocalEndPoint];

                ListenerInfo info = new ListenerInfo();
                
                info.StartDelegate = acceptor;
                info.Parameters = parameters;

                if (theListener._applications.ContainsKey(parameters.CalledAE))
                {
                    DicomLogger.LogError("Already listening with AE {0} on {1}", parameters.CalledAE, parameters.LocalEndPoint.ToString());
                    return false;
                }

                theListener._applications.Add(parameters.CalledAE,info);
                DicomLogger.LogInfo("Starting to listen with AE {0} on existing port {1}", parameters.CalledAE, parameters.LocalEndPoint.ToString());
            }
            else
            {
                
                Listener theListener = new Listener(parameters, acceptor);
                _listeners[parameters.LocalEndPoint] = theListener;
                theListener.StartThread();

                DicomLogger.LogInfo("Starting to listen with AE {0} on port {1}", parameters.CalledAE, parameters.LocalEndPoint.ToString());
            }

            return true;
        }

        public static bool StopListening(ServerAssociationParameters parameters)
        {
            Listener theListener;

            if (_listeners.ContainsKey(parameters.LocalEndPoint))
            {
                theListener = _listeners[parameters.LocalEndPoint];

                if (theListener._applications.ContainsKey(parameters.CalledAE))
                {
                    theListener._applications.Remove(parameters.CalledAE);

                    if (theListener._applications.Count == 0)
                    {
                        // Cleanup the listener
                        _listeners.Remove(parameters.LocalEndPoint);
                        theListener.StopThread();
                        theListener.Dispose();
                    }
                    DicomLogger.LogInfo("Stopping listening wiith AE {0} on {1}", parameters.CalledAE, parameters.LocalEndPoint.ToString());
                }
                else
                {
                    DicomLogger.LogError("Unable to stop listening on AE {0}, assembly was not listening with this AE.", parameters.CalledAE);
                    return false;
                }
            }
            else
            {
                DicomLogger.LogError("Unable to stop listening, assembly was not listening on end point {0}.", parameters.LocalEndPoint.ToString());
                return false;
            }

            return true;
        }
        #endregion

        #region Constructors
        internal Listener(ServerAssociationParameters parameters, StartAssociation acceptor)
        {

            ListenerInfo info = new ListenerInfo();

            info.Parameters = parameters;
            info.StartDelegate = acceptor;

            this._applications.Add(parameters.CalledAE, info);

            _ipEndPoint = parameters.LocalEndPoint;
        }
        #endregion

        private void StartThread()
        {
            
            _theThread = new Thread(new ThreadStart(Listen));
            _theThread.Name = "Association Listener on port " + _ipEndPoint.Port;

            _theThread.Start();
        }

        private void StopThread()
        {
            _stop = true;

            _theThread.Join();
        }

        public void Listen()
        {
            _tcpListener = new TcpListener(_ipEndPoint);
            try
            {
                _tcpListener.Start(50);
            }
            catch (SocketException e)
            {
                DicomLogger.LogErrorException(e,"Unexpected exception when starting TCP listener");
                DicomLogger.LogError("Shutting down listener on {0}", _ipEndPoint.ToString());
                return;
            }

            while (_stop == false)
            {
                // Tried Async i/o here, but had some weird problems with connections not getting
                // through.
                if (_tcpListener.Pending())
                {
                    Socket theSocket = _tcpListener.AcceptSocket();
                    DicomServer server = new DicomServer(theSocket, _applications);
                    continue;
                }
                Thread.Sleep(10);               
            }
            try
            {
                _tcpListener.Stop();
            }
            catch (SocketException e)
            {
                DicomLogger.LogErrorException(e, "Unexpected exception when stoppinging TCP listener on {0}",_ipEndPoint,ToString());
            }
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
