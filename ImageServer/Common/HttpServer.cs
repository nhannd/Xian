using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Represents the argument associated with the event fired when a http request is received.
    /// </summary>
    public class HttpRequestReceivedEventArg : EventArgs
    {
        private HttpListenerContext _context;

        public HttpRequestReceivedEventArg(HttpListenerContext context)
        {
            _context = context;
        }

        public HttpListenerContext Context
        {
            get { return _context; }
            set { _context = value; }
        }
    }


    /// <summary>
    /// Represents a http server that accepts and processes http requests.
    /// </summary>
    public abstract class HttpServer : HttpListenerShred
    {
        
        #region Public Delegates
        public delegate void HttpListenerHandlerDelegate(object sender, HttpRequestReceivedEventArg args);
        #endregion

        #region Private Members
        private event HttpListenerHandlerDelegate _httpRequestReceived;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when a http request is received.
        /// </summary>
        public event HttpListenerHandlerDelegate HttpRequestReceived
        {
            add { _httpRequestReceived += value; }
            remove { _httpRequestReceived -= value; }
        }
        
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="HttpServer"/> on a specified address.
        /// </summary>
        /// <param name="uri"></param>
        public HttpServer(string uri) : base(uri)
        {
        }
        #endregion

        #region Overridden Public Methods

        public override void Start()
        {
            StartListening(ListenerCallback);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Handles incoming http request asynchronously
        /// </summary>
        /// <param name="result"></param>
        private void ListenerCallback(IAsyncResult result)
        {
            HttpListenerContext context = null;

            try
            {
                HttpListenerAsyncState state = result.AsyncState as HttpListenerAsyncState;
                if (state != null)
                {
                    HttpListener listener = state.Listener;
                    if (listener.IsListening)
                    {
                        context = listener.EndGetContext(result);

                        Thread.CurrentThread.Name = String.Format("Http Request(from {0}:{1})", context.Request.RemoteEndPoint.Address, context.Request.RemoteEndPoint.Port);

                        Platform.Log(LogLevel.Debug, "Handling http request");

                        // signal the listener that it can now accept another connection
                        state.WaitEvent.Set();

                        EventsHelper.Fire(_httpRequestReceived, this, new HttpRequestReceivedEventArg(context));
                        
                    }

                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Error while handling http request:");

                if (context!=null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.StatusDescription = e.Message;
                }
                
            }
            finally
            {
                if (context != null)
                {
                    if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                        Platform.Log(LogLevel.Debug, "Request completed successfully");
                    else
                        Platform.Log(LogLevel.Error, "{0}:{1}", context.Response.StatusCode, context.Response.StatusDescription);

                    try
                    {
                        context.Response.Close();

                    }catch(Exception)
                    {
                    }
                }

            }

        }
        #endregion

    }
}


