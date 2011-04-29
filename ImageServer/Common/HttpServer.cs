#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Web;
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
        public HttpRequestReceivedEventArg(HttpListenerContext context)
        {
            Context = context;
        }

        public HttpListenerContext Context { get; set; }
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

        private readonly string _name;
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
        /// <param name="serverName">Name of the Http server</param>
        /// <param name="port"></param>
        /// <param name="path"></param>
        protected HttpServer(string serverName, int port, string path) : 
            base(port, path)
        {
            _name = serverName;
        }
        #endregion

        #region Overridden Public Methods

        public override void Start()
        {
            try
            {
                StartListening(ListenerCallback);
            }
            catch(HttpListenerException e)
            {
                // When the port is tied up by another process, the system throws HttpListenerException with error code = 32 
                // and the message "The process cannot access the file because it is being used by another process". 
                // For clarity, we make the error message more informative in this case
                if (e.ErrorCode == WindowsErrorCodes.ERROR_SHARING_VIOLATION)
                {
                    string errorMessage = string.Format("Unable to start {0} on port {1}. The port is being used by another process", _name, Port);
                    Platform.Log(LogLevel.Fatal, errorMessage);
                    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, _name, AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, errorMessage);
                }
                else
                {
                    string errorMessage = string.Format("Unable to start {0}. System Error Code={1}", _name, e.ErrorCode);
                    Platform.Log(LogLevel.Fatal, e, errorMessage);
                    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, _name, AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, errorMessage);
                }
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unable to start {0}", _name);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, _name, AlertTypeCodes.UnableToStart,
                                        null, TimeSpan.Zero, "Unable to start {0}: {1}", _name, e.Message);
            }
            
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
                        if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                        {
                            Platform.Log(LogLevel.Debug, "Handling http request");
                            Platform.Log(LogLevel.Debug, "{0}", context.Request.Url.AbsoluteUri);
                        }

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
                    try
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.StatusDescription = e.InnerException != null ? HttpUtility.HtmlEncode(e.InnerException.Message) : HttpUtility.HtmlEncode(e.Message);
                    }
                    catch(Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Unable to set response status description");
                    }
                }
                
            }

        }
        #endregion

    }
}


