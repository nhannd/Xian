using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// Represents the state of the asynchronous operation that occurs when a http request is processed.
    /// </summary>
    public class HttpListenerAsyncState
    {
        #region Private Members
        private HttpListener _listener;
        private ManualResetEvent _waitEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the wait event associated with the operation
        /// </summary>
        public ManualResetEvent WaitEvent
        {
            get { return _waitEvent; }
        }

        /// <summary>
        /// Gets the http listener object
        /// </summary>
        public HttpListener Listener
        {
            get { return _listener; }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates an instance of <see cref="HttpListenerAsyncState"/> for the specified http listener.
        /// </summary>
        /// <param name="listener"></param>
        public HttpListenerAsyncState(HttpListener listener)
        {
            _listener = listener;
            _waitEvent = new ManualResetEvent(false);
        }

        #endregion
    }

    /// <summary>
    /// Represents a shred that listens to and handles http requests.
    /// </summary>
    public abstract class HttpListenerShred : Shred
    {
        #region Private Members
        private HttpListener _listener;
        private string _uri;
        private string _name;
        private List<ManualResetEvent> _handlers = new List<ManualResetEvent>();
        private HttpListenerAsyncState _syncState;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="HttpListenerShred"/> to listern at the specified address.
        /// </summary>
        /// <param name="uri">The URI where the listen will listens at</param>
        public HttpListenerShred(string uri)
        {
            _uri = uri;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the URI where the shred is listening at for incoming http requests.
        /// </summary>
        public string Uri
        {
            get { return _uri; }
        }

        /// <summary>
        /// Gets or sets the name of the shred.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Protected Methods

        protected void StartListening(AsyncCallback callback)
        {
            _listener = new HttpListener(); ;
            _listener.Prefixes.Add(Uri);
            _listener.Start();

            Platform.Log(LogLevel.Info, "Started listening at {0}", Uri);

            while (_listener.IsListening)
            {
                Platform.Log(LogLevel.Debug, "Waiting for request at {0}", Uri);

                _syncState = new HttpListenerAsyncState(_listener);

                _listener.BeginGetContext(callback, _syncState);

                _syncState.WaitEvent.WaitOne();

            }
        }

        #endregion


        #region Overridden Public Methods

        public override void Stop()
        {
            _listener.Stop();
            _syncState.WaitEvent.Set();

        }

        #endregion

    }
}
