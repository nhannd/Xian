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
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
    internal static class UriHelper
    {
        private static int SegmentCount
        {
            get
            {
                String testUrl = String.Format("http://localhost:{0}{1}", ImageStreamingServerSettings.Default.Port, ImageStreamingServerSettings.Default.Path);
                UriBuilder builder = new UriBuilder(testUrl);
                return builder.Uri.Segments.Length;
            }
        }

        public static string GetServerAE(HttpListenerContext context)
        {
            string[] requestSegments = context.Request.Url.Segments;
            if (requestSegments.Length <= SegmentCount)
                return String.Empty;

            return requestSegments[SegmentCount];
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
        #endregion

	    		
        #region Constructor

        /// <summary>
	    /// Creates an instance of <see cref="ImageStreamingServer"/>
	    /// </summary>
	    public ImageStreamingServer()
	        : base(SR.ImageStreamingServerDisplayName,
                ImageStreamingServerSettings.Default.Port,
                ImageStreamingServerSettings.Default.Path)
		{
            HttpRequestReceived += OnHttpRequestReceived;
            	
		}

        
	    #endregion

        #region Static Methods

        
        private static void Validate(HttpListenerContext context)
        {
            string serverAE = UriHelper.GetServerAE(context);

            if (String.IsNullOrEmpty(serverAE))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, 
                    String.Format("Partition AE Title is required after {0}", ImageStreamingServerSettings.Default.Path));
            }


            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(serverAE);
            if (partition == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("Partition AE {0} is invalid", serverAE));
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

                WADORequestProcessor processor = new WADORequestProcessor();
                processor.Process(context);
				
			}
            catch (WADOException e)
            {
                context.Response.StatusCode = e.GetHttpCode();
                context.Response.StatusDescription = e.Message;
            }
            catch (HttpException e)
            {
                context.Response.StatusCode = e.GetHttpCode();
                context.Response.StatusDescription = e.Message;
            }
            catch (ServerTransientError e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NoContent;
                context.Response.StatusDescription = e.Message;
            }
            catch(StudyNotFoundException e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusDescription = e.Message;
            }
            catch (Exception e)
			{
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                
                if (e.InnerException!=null)
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.InnerException.Message);
				else
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.Message);

			    Platform.Log(LogLevel.Error, e);
                
			}
            finally
			{
               
                    // note: the connection might have been aborted or lost, the following
                    try
                    {
                        context.Response.OutputStream.Flush();
                        context.Response.OutputStream.Close();
                    }
                    finally
                    {
                        RemoveContext(context);
                    }
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