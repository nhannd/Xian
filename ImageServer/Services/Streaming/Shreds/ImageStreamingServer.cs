using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
    internal static class UriHelper
    {
        public static Uri GetConfiguredUri()
        {
            ImageStreamingServerSettings settings = ImageStreamingServerSettings.Default;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = Uri.UriSchemeHttp;
            uriBuilder.Host = "localhost";
            uriBuilder.Port = settings.Port;
            uriBuilder.Path = settings.Path;
            return uriBuilder.Uri;
        }


        public static string GetServerAE(HttpListenerContext context)
        {
            Uri listeningUri = GetConfiguredUri();
            if (context.Request.Url.AbsolutePath.Length > listeningUri.AbsolutePath.Length)
            {
                string serverAE = context.Request.Url.AbsolutePath.Substring(listeningUri.AbsolutePath.Length);
                return serverAE;
            }
            else
                return String.Empty;
        }
    }

	/// <summary>
	/// Represents an image streaming server.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageStreamingServer : HttpServer
    {
        #region Private Fields
        private readonly List<IPEndPoint> _currentRequests = new List<IPEndPoint>();
        private WADORequestProcessor _processor;
		#endregion

	    		
        #region Constructor

        /// <summary>
	    /// Creates an instance of <see cref="ImageStreamingServer"/>
	    /// </summary>
	    public ImageStreamingServer()
	        : base(SR.ImageStreamingServerDisplayName,
                UriHelper.GetConfiguredUri())
		{
			HttpRequestReceived += OnHttpRequestReceived;
            _processor = new WADORequestProcessor();
				
		}

        
	    #endregion

        #region Static Methods

        
        private static void Validate(HttpListenerContext context)
        {
            Uri listeningUri = UriHelper.GetConfiguredUri();
            string serverAE = UriHelper.GetServerAE(context);

            if (String.IsNullOrEmpty(serverAE))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("Partition AE Title is required after {0}", listeningUri));
            }


            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(serverAE);
            if (partition == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("Partition AE {0} is invalid", serverAE));
            }

            if (!partition.Enabled)
            {
                throw new WADOException(HttpStatusCode.Forbidden, String.Format("Partition {0} is disabled", serverAE));
            }

            string requestType = context.Request.QueryString["requestType"] ?? "";
            if (String.IsNullOrEmpty(requestType))
            {
                throw new WADOException(HttpStatusCode.BadRequest, "RequestType parameter is missing");
            }

        }

        #endregion

        #region Private Methods
        void AddContext(HttpListenerContext ctx)
        {
            //Platform.Log(LogLevel.Info, ctx.Request.Url.Query);
                    
            lock (_currentRequests)
            {
                _currentRequests.Add(ctx.Request.RemoteEndPoint);
                if (_currentRequests.Count > ImageStreamingServerSettings.Default.ConcurrencyWarningThreshold)
                {
                    StringBuilder log = new StringBuilder();
                    log.AppendLine(String.Format("Concurrency threshold detected: {0} requests are being processed.", _currentRequests.Count));
                    Dictionary<IPAddress, List<IPEndPoint>> map = CollectionUtils.GroupBy<IPEndPoint, IPAddress>(_currentRequests, delegate(IPEndPoint item) { return item.Address; });
                    foreach (IPAddress client in map.Keys)
                    {
                        log.AppendLine(String.Format("From {0} : {1}", client, map[client].Count));
                    }
                    Platform.Log(LogLevel.Warn, log.ToString());
                }
            }
        }

        void RemoveContext(HttpListenerContext ctx)
        {
            lock (_currentRequests)
            {
                _currentRequests.Remove(ctx.Request.RemoteEndPoint);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
		/// Event handler for <see cref="HttpServer.HttpRequestReceived"/> events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected void OnHttpRequestReceived(object sender, HttpRequestReceivedEventArg args)
		{

			// NOTE: This method is run under different threads for different http requests.
            HttpListenerContext context = args.Context;
            Thread.CurrentThread.Name = String.Format("Streaming (client: {0}:{1})", context.Request.RemoteEndPoint.Address, context.Request.RemoteEndPoint.Port);

		    AddContext(context);

			try
			{
			    Validate(context);
                _processor.Process(context);
				
			}
			catch (HttpException e)
			{
				context.Response.StatusCode = e.GetHttpCode();
				if (e.InnerException!=null)
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.InnerException.Message);
				else
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.Message);
                
			}
            finally
			{
			    RemoveContext(context);
			}

		}

		#endregion

        #region Overridden Public Methods


        public override string GetDisplayName()
		{
			return SR.ImageStreamingServerDescription;
		}

		public override string GetDescription()
		{
			return SR.ImageStreamingServerDescription;
		}

		#endregion

	}
}