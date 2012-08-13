#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Ris.Application.Common.Printing
{
	public class PrintJob
	{
		class HttpServer
		{
			private readonly HttpListener _httpListener;
			private Thread _listenerThread;

			public HttpServer()
			{
				_httpListener = new HttpListener();
				_httpListener.Prefixes.Add(_proxyHost);
			}

			public void Start()
			{
				if (_httpListener.IsListening)
					return;

				_httpListener.Start();

				_listenerThread = new Thread(Listen) {IsBackground = true};
				_listenerThread.Start();
			}

			public void Stop()
			{
				_httpListener.Stop();
			}

			private void Listen(object state)
			{
				while (_httpListener.IsListening)
				{
					var context = _httpListener.GetContext();
					ThreadPool.QueueUserWorkItem(ProcessRequest, context);
				}
			}

			private void ProcessRequest(object state)
			{
				var httpContext = (HttpListenerContext)state;

				try
				{
					if(!HandleRequest(httpContext))
					{
						Redirect(httpContext);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					Error(httpContext.Response, 500, "Internal Server Error");
				}
				finally
				{
					// always close output stream, or the response will never terminate (even in an error condition)
					httpContext.Response.OutputStream.Close();
				}
			}

			private bool HandleRequest(HttpListenerContext httpContext)
			{
				var query = ParseQueryString(httpContext.Request.Url);
				var id = query["id"];
				if (string.IsNullOrEmpty(id))
					return false;

				var job = _runningJobs[new Guid(id)];

				// we only handle request to our original url
				if (httpContext.Request.Url.AbsolutePath != job.TemplateUrl.AbsolutePath)
					return false;

				using(var writer = new StreamWriter(httpContext.Response.OutputStream))
				{
					job.WriteHtml(writer);
				}
				return true;
			}

			private static void Redirect(HttpListenerContext httpListenerContext)
			{
				var url = httpListenerContext.Request.Url;
				var redirectUrl = new Uri(new Uri(_mainHost), url.AbsolutePath);
				httpListenerContext.Response.Redirect(redirectUrl.ToString());
			}

			private static void Error(HttpListenerResponse response, int code, string message)
			{
				response.StatusCode = code;
				response.StatusDescription = message;
			}

			private NameValueCollection ParseQueryString(Uri url)
			{
				return string.IsNullOrEmpty(url.Query) ? new NameValueCollection() : HttpUtility.ParseQueryString(url.Query);
			}
		}

		public class Result : IDisposable
		{
			internal Result(string outputFilePath)
			{
				OutputFilePath = outputFilePath;
			}

			public string OutputFilePath { get; private set; }

			public void Dispose()
			{
				File.Delete(this.OutputFilePath);
			}
		}

		private static string _mainHost = "http://localhost";
		private static string _proxyHost = "http://localhost:55355/";
		private static readonly Dictionary<Guid, PrintJob> _runningJobs = new Dictionary<Guid, PrintJob>();
		private static readonly HttpServer _httpServer = new HttpServer();

		static PrintJob()
		{
			try
			{
				Platform.Log(LogLevel.Info, "Starting print server...");
				_httpServer.Start();
				Platform.Log(LogLevel.Info, "Print server started.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}


		public static Result Run(string url, Dictionary<string, object> data)
		{
			var uri = new Uri(url);
			if(!uri.IsLoopback)
				throw new ArgumentException("Must be a local address");
			if(!url.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) && !url.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("Must be an html file");

			var guid = Guid.NewGuid();
			var job = new PrintJob(guid, uri, data);
			return job.Run();
		}

		private readonly Guid _id;
		private readonly Uri _url;
		private readonly Dictionary<string, object> _data;

		private PrintJob(Guid id, Uri url, Dictionary<string, object> data)
		{
			_id = id;
			_url = url;
			_data = data;
		}

		private Uri TemplateUrl
		{
			get { return _url; }
		}

		private Result Run()
		{
			var outputFilePath = Path.GetTempFileName();

			lock (_runningJobs)
			{
				_runningJobs.Add(_id, this);
			}

			try
			{
				RunWkHtml(outputFilePath);
			}
			finally
			{
				lock (_runningJobs)
				{
					_runningJobs.Remove(_id);
				}
			}
			return new Result(outputFilePath);
		}

		private void WriteHtml(TextWriter writer)
		{
			var request = WebRequest.Create(_url);
			var response = request.GetResponse();
			using (var s = response.GetResponseStream())
			{
				using (var reader = new StreamReader(s))
				{
					var template = new ActiveTemplate(reader);
					var html = template.Evaluate(_data);
					writer.Write(html);
				}
			}
		}

		private void RunWkHtml(string outputFilePath)
		{
			var sourcePath = string.Format("{0}?id={1}", _url.AbsolutePath, _id.ToString("N"));
			var sourceUrl = new Uri(new Uri(_proxyHost), sourcePath);

			var startInfo = new ProcessStartInfo("wkhtmltopdf", string.Format("{0} {1}", sourceUrl, outputFilePath));
			startInfo.UseShellExecute = false;
			var process = Process.Start(startInfo);
			process.WaitForExit();	//todo: include a time-out
		}
	}
}
