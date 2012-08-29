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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Enterprise.Core.Printing
{
	public class PrintJob
	{
		#region HttpServer class

		class HttpServer
		{
			private readonly string _host;
			private readonly HttpListener _httpListener;
			private Thread _listenerThread;

			public HttpServer(string host)
			{
				_host = host;
				_httpListener = new HttpListener();
				_httpListener.Prefixes.Add(host);
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

			public bool IsStarted
			{
				get { return _httpListener.IsListening; }
			}

			public string Host
			{
				get { return _host; }
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

				Page page;
				lock(_runningJobs)
				{
					page = _runningJobs[new Guid(id)];
				}

				// we only handle request to our original url
				if (httpContext.Request.Url.AbsolutePath != page.TemplateUrl.AbsolutePath)
					return false;

				Platform.Log(LogLevel.Info, "Received print request: {0}", httpContext.Request.Url);

				// tell the browser that our response will be UTF-8
				httpContext.Response.Headers[HttpResponseHeader.ContentType] = "text/html;charset=UTF-8";

				// write response
				using(var writer = new StreamWriter(httpContext.Response.OutputStream))
				{
					page.WriteHtml(writer);
				}
				return true;
			}

			private static void Redirect(HttpListenerContext httpListenerContext)
			{
				var url = httpListenerContext.Request.Url;
				var redirectUrl = new Uri(new Uri(LocalHost), url.AbsolutePath);
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

		#endregion

		#region Result class

		public class Result : IDisposable
		{
			public Result(string outputFilePath)
			{
				OutputFilePath = outputFilePath;
			}

			public string OutputFilePath { get; private set; }

			public void Dispose()
			{
				if(!string.IsNullOrEmpty(this.OutputFilePath))
				{
					File.Delete(this.OutputFilePath);
				}
			}
		}

		#endregion

		#region Page class

		class Page
		{
			private readonly PrintJob _job;
			private readonly Guid _id;
			private readonly Uri _templateUri;
			private readonly Dictionary<string, object> _variables; 

			internal Page(PrintJob job, Guid id, IPageModel pageModel)
			{
				_job = job;
				_id = id;
				_templateUri = pageModel.TemplateUrl;
				_variables = pageModel.Variables;
			}

			public Guid Id
			{
				get { return _id; }
			}

			public Uri TemplateUrl
			{
				get { return _templateUri; }
			}

			public Uri ConverterUrl
			{
				get
				{
					var sourcePath = string.Format("{0}?id={1}", TemplateUrl.AbsolutePath, _id.ToString("N"));
					return new Uri(new Uri(_httpServer.Host), sourcePath);
				}
			}

			public void WriteHtml(TextWriter writer)
			{
				try
				{
					var request = WebRequest.Create(TemplateUrl);
					var response = (HttpWebResponse)request.GetResponse();
					using (var s = response.GetResponseStream())
					{
						// doubt that GetResponseStream ever returns null, but just in case
						if (s == null)
						{
							_job.SetError("No response stream available.");
							return;
						}

						using (var reader = new StreamReader(s, GetEncoding(response.CharacterSet)))
						{
							var template = new ActiveTemplate(reader);
							var html = template.Evaluate(_variables);
							writer.Write(html);
						}
					}
				}
				catch (WebException e)
				{
					// explicitly handle 404 to provide a helpful error message
					if (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
					{
						_job.SetError(string.Format("The template {0} was not found (404).", TemplateUrl));
					}
					else
					{
						_job.SetError(e.Message);
					}
					throw;
				}
				catch (Exception e)
				{
					_job.SetError(e.Message);
					throw;
				}
			}

			private Encoding GetEncoding(string enc)
			{
				try
				{
					return Encoding.GetEncoding(enc);
				}
				catch (Exception)
				{
					return Encoding.Default;
				}
			}

		}

		#endregion

		private const string LocalHost = "http://localhost";
		private static readonly Dictionary<Guid, Page> _runningJobs = new Dictionary<Guid, Page>();
		private static readonly HttpServer _httpServer;

		static PrintJob()
		{
			try
			{
				var settings = new PrintSettings();
				var portRange = GetPortRange(settings.HttpProxyServerPortRange);
				Platform.Log(LogLevel.Info, "Starting print server...");

				foreach (var port in portRange)
				{
					if(TryStartHttpServer(port, out _httpServer))
					{
						Platform.Log(LogLevel.Info, "Print server started on port {0}.", port);
						return;
					}
				}
				Platform.Log(LogLevel.Error, "Unable to start print server on any port in range {0}.", portRange);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		/// <summary>
		/// Creates and executes a single-page print job.
		/// </summary>
		/// <param name="pageModel"></param>
		/// <returns></returns>
		public static Result Run(IPageModel pageModel)
		{
			return Run(new[] {pageModel});
		}

		/// <summary>
		/// Creates and executes a multi-page print job.
		/// </summary>
		/// <param name="pageModels"></param>
		/// <returns></returns>
		public static Result Run(IList<IPageModel> pageModels)
		{
			// validate input
			foreach (var pageModel in pageModels)
			{
				if (!pageModel.TemplateUrl.IsLoopback)
					throw new ArgumentException("Must be a local address");

				var url = pageModel.TemplateUrl.ToString();
				if (!url.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) &&
					!url.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
					throw new ArgumentException("Must be an html file");
			}

			var job = new PrintJob(pageModels);
			return job.Run();
		}

		private readonly IList<Page> _pages; 
		private bool _error;
		private string _errorMessage;

		private PrintJob(IEnumerable<IPageModel> pageModels)
		{
			_pages = pageModels.Select(m => new Page(this, Guid.NewGuid(), m)).ToList();
		}

		private Result Run()
		{
			if(_httpServer == null || !_httpServer.IsStarted)
				throw new PrintException("The print HTTP server could not be started.");

			var outputFilePath = Path.GetTempFileName();

			lock (_runningJobs)
			{
				foreach (var page in _pages)
					_runningJobs.Add(page.Id, page);
			}

			try
			{
				// try this up to five times
				for (var i = 0; i < 5; i++)
				{
					_error = false;
					RunConverter(outputFilePath);
					if (!_error)
						break;
				}
			}
			finally
			{
				lock (_runningJobs)
				{
					foreach (var page in _pages)
						_runningJobs.Remove(page.Id);
				}
				if(_error)
				{
					File.Delete(outputFilePath);
				}
			}

			if(_error)
				throw new PrintException(_errorMessage);

			return new Result(outputFilePath);
		}


		private void RunConverter(string outputFilePath)
		{
			var settings = new PrintSettings();
			try
			{
				var sourceUrls = string.Join(" ", _pages.Select(p => p.ConverterUrl.ToString()));
				var arguments = string.Format("{0} {1} {2}", settings.ConverterOptions, sourceUrls, outputFilePath);
				var startInfo = new ProcessStartInfo(settings.ConverterProgram, arguments) {UseShellExecute = false};
				var process = Process.Start(startInfo);
				var exited = process.WaitForExit(settings.ConverterTimeout * 1000);

				if (!exited)
				{
					SetError(string.Format("The converter program ({0}) timed out after {1} ms - printing aborted.",
						settings.ConverterProgram, settings.ConverterTimeout));
				}

			}
			catch (Win32Exception e)
			{
				SetError(
					string.Format("{0}. Usually this means the converter program is not installed, or its location ({1}) is incorrectly configured.",
					e.Message,
					settings.ConverterProgram)
				);
			}

			//var exitCode = process.ExitCode;
			//Console.WriteLine("EXITCODE = " + exitCode);

			//var startInfo = new ProcessStartInfo(sourceUrl.ToString());
			//var process = Process.Start(startInfo);
			//Thread.Sleep(10000);
		}

		private void SetError(string message)
		{
			_error = true;
			_errorMessage = message;
		}

		private static bool TryStartHttpServer(int port, out HttpServer server)
		{
			try
			{
				server = new HttpServer(string.Format("{0}:{1}/", LocalHost, port));
				server.Start();
				return true;
			}
			catch (HttpListenerException)
			{
				server = null;
				return false;
			}
		}

		private static IEnumerable<int> GetPortRange(string portRangeString)
		{
			if (!string.IsNullOrEmpty(portRangeString))
			{
				var match = Regex.Match(portRangeString, @"^\s*(\d+)\s*\-\s*(\d+)\s*$");
				if (match.Success)
				{
					var lower = int.Parse(match.Groups[1].Value);
					var upper = int.Parse(match.Groups[2].Value);
					if (upper > lower)
					{
						return Enumerable.Range(lower, upper - lower + 1);
					}
				}
			}
			throw new PrintException("PrintSettings.HttpProxyServerPortRange must specify a valid port range in the form e.g. 10000-10005.");
		}
	}
}
