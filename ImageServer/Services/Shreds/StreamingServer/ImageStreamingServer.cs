using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Shreds.StreamingServer
{
    /// <summary>
    /// Represents an image streaming server.
    /// </summary>
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class ImageStreamingServer : HttpServer
    {
        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="ImageStreamingServer"/>
        /// </summary>
        public ImageStreamingServer()
            : base(ImageStreamingServerSettings.Default.Address)
        {
            HttpRequestReceived += OnHttpRequestReceived;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Event handler for <see cref="HttpServer.HttpRequestReceived"/> events.
        /// </summary>
        /// <param name="args"></param>
        protected void OnHttpRequestReceived(object sender, HttpRequestReceivedEventArg args)
        {
            // NOTE: This method is run under different threads for different http requests.

            HttpListenerContext context = args.Context;

            try
            {
                WADORequestProcessor processor = new WADORequestProcessor();
                processor.Process(context);
            }
            catch (WADOException e)
            {
                context.Response.StatusCode = e.HttpErrorCode;
                context.Response.StatusDescription = e.Message;
            }

        }

        #endregion


        #region Overridden Public Methods

        public override void Stop()
        {
            // TODO: how wait for all threads to finish?

            base.Stop();
        }


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
