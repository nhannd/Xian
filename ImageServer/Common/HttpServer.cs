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
        /// <param name="uri">The Uri where the server will listen at</param>
        public HttpServer(string serverName, Uri uri) : base(uri)
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
            catch(Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unable to start {0}", _name);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, _name, AlertTypeCodes.UnableToStart, "Unable to start {0}: {1}", _name, e.Message);
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
                        Platform.Log(LogLevel.Debug, "Handling http request");
                        Platform.Log(LogLevel.Debug, "{0}", context.Request.Url.AbsoluteUri);
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
                        if (e.InnerException != null)
                            context.Response.StatusDescription = HttpUtility.HtmlEncode(e.InnerException.Message);
                        else
                            context.Response.StatusDescription = HttpUtility.HtmlEncode(e.Message);
                    }
                    catch(Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Unable to set response status description");
                    }
                }
                
            }
            finally
            {
                //if (context != null)
                //{
                //    if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                //        Platform.Log(LogLevel.Debug, "Request completed successfully");
                //    else
                //        Platform.Log(LogLevel.Error, "{0}:{1}", context.Response.StatusCode, HttpUtility.HtmlDecode(context.Response.StatusDescription));

                //    try
                //    {
                //        context.Response.Close();

                //    }catch(Exception ex)
                //    {
                //        Platform.Log(LogLevel.Error, ex);
                //    }
                //}

            }

        }
        #endregion

    }
}


