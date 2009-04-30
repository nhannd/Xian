#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Net;
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
        private readonly Uri _uri;
        private string _name;
        private HttpListenerAsyncState _syncState;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="HttpListenerShred"/> to listern at the specified address.
        /// </summary>
        /// <param name="uri">The URI where the listen will listens at</param>
        public HttpListenerShred(Uri uri)
        {
            _uri = uri;
        }

        #endregion


        #region Public Properties


        /// <summary>
        /// Gets the URI where the shred is listening at for incoming http requests.
        /// </summary>
        public Uri BaseUri
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
            Platform.Log(LogLevel.Info, "Started listening at {0}", BaseUri);

            _listener = new HttpListener(); ;
            _listener.Prefixes.Add(BaseUri.AbsoluteUri);
            _listener.Start();

            
            while (_listener.IsListening)
            {
                Platform.Log(LogLevel.Debug, "Waiting for request at {0}", BaseUri);

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
