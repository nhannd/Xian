using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom
{
    public class Listener
    {
        #region Members

        static private Dictionary<int, Listener> _listeners = new Dictionary<int, Listener>();
        private int listenPort;
        private Dictionary<String, ApplicationEntity> _applications = new Dictionary<String,ApplicationEntity>();
        private TcpListener tcpListener = null;
        private Thread theThread = null;
        #endregion

        #region Public Static Methods

        public static void StartListening(int port, ApplicationEntity ae, IApplicationAcceptor acceptor)
        {

        }

      

        internal Listener(int port, ApplicationEntity ae)
        {
            Listener listener = _listeners[port];

            if (listener == null)
            {
                listener = new Listener(port, ae);
                _listeners[port] = listener;

                listener.theThread.Start();
            }
            else
            {
                ApplicationEntity aeCheck;

                aeCheck = listener._applications[ae.Name];
                if (aeCheck != null)
                {
                    //CCDlog.Warning("Replacing AE title for listener on port " + port);
                }

                listener._applications[ae.Name] = ae;
            }

        }

        public static void StopListening(int port, ApplicationEntity ae)
        {
            Listener listener;

            listener = _listeners[port];
            if (listener != null)
            {
                ApplicationEntity aeCheck;

                aeCheck = listener._applications[ae.Name];
                if (aeCheck != null)
                {
                    listener._applications[ae.Name] = null;

                    if (listener._applications.Count == 0)
                    {
                        _listeners[port] = null;


                    }
                }
            }

        }
        #endregion

/*
        private Listener(int port, ApplicationEntity ae)
        {
            listenPort = port;

            tcpListener = new TcpListener(IPAddress.Any,port);

            _applications[ae.Name] = ae;

            theThread = new Thread(new ThreadStart(Listen));
        }

        */
        public void Listen()
        {
            tcpListener.Start();

            while (true)
            {
                tcpListener.BeginAcceptTcpClient(
                    new AsyncCallback(DoBeginAcceptCallback), this);
            }
        }

        private void DoBeginAcceptCallback(IAsyncResult ar)
        {
        }
    }
}
