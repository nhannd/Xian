using System;
using System.Net;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
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
			: base(SR.ImageStreamingServerDisplayName, ImageStreamingServerSettings.Default.Address)
		{
			HttpRequestReceived += OnHttpRequestReceived;
            
		}


		#endregion

  

		#region Protected Methods

		/// <summary>
		/// Event handler for <see cref="HttpServer.HttpRequestReceived"/> events.
		/// </summary>
		/// <param name="args"></param>
		protected static void OnHttpRequestReceived(object sender, HttpRequestReceivedEventArg args)
		{
			// NOTE: This method is run under different threads for different http requests.

			HttpListenerContext context = args.Context;

			try
			{
				//TODO: find better way to "map" the requests to the processor
				if (context.Request.Url.Segments.Length>1)
				{
					if (context.Request.Url.Segments[1].Equals("WADO/", StringComparison.InvariantCultureIgnoreCase))
					{
						WADORequestProcessor processor = new WADORequestProcessor();
						processor.Process(context);
					}
					else
					{
						throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid url");
					}
				}
			}
			catch (HttpException e)
			{
				context.Response.StatusCode = e.GetHttpCode();
				if (e.InnerException!=null)
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.InnerException.Message);
				else
					context.Response.StatusDescription = HttpUtility.HtmlEncode(e.Message);
                
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