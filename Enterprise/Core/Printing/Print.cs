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
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Enterprise.Core.Printing
{
	public class PrintJob
	{
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

				PrintJob job;
				lock(_runningJobs)
				{
					job = _runningJobs[new Guid(id)];
				}

				// we only handle request to our original url
				if (httpContext.Request.Url.AbsolutePath != job.TemplateUrl.AbsolutePath)
					return false;

				Platform.Log(LogLevel.Info, "Received print request: {0}", httpContext.Request.Url);

				// tell the browser that our response will be UTF-8
				httpContext.Response.Headers[HttpResponseHeader.ContentType] = "text/html;charset=UTF-8";

				// write response
				using(var writer = new StreamWriter(httpContext.Response.OutputStream))
				{
					job.WriteHtml(writer);
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

		private const string LocalHost = "http://localhost";
		private static readonly Dictionary<Guid, PrintJob> _runningJobs = new Dictionary<Guid, PrintJob>();
		private static readonly HttpServer _httpServer;

		static PrintJob()
		{
			try
			{
				Platform.Log(LogLevel.Info, "Starting print server...");
				_httpServer = new HttpServer(string.Format("{0}:{1}/", LocalHost, new PrintSettings().HttpProxyServerPort));
				_httpServer.Start();
				Platform.Log(LogLevel.Info, "Print server started.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}


		public static Result Run(IPrintModel printModel)
		{
			if (!printModel.TemplateUrl.IsLoopback)
				throw new ArgumentException("Must be a local address");

			var url = printModel.TemplateUrl.ToString();
			if(!url.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) && !url.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("Must be an html file");

			var guid = Guid.NewGuid();
			var job = new PrintJob(guid, printModel.TemplateUrl, printModel.Variables);
			return job.Run();
		}

		private readonly Guid _id;
		private readonly Uri _url;
		private readonly Dictionary<string, object> _data;
		private bool _error;
		private string _errorMessage;

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
			if(_httpServer == null || !_httpServer.IsStarted)
				throw new PrintException("The print HTTP server could not be started.");

			var outputFilePath = Path.GetTempFileName();

			lock (_runningJobs)
			{
				_runningJobs.Add(_id, this);
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
					_runningJobs.Remove(_id);
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

		private void WriteHtml(TextWriter writer)
		{
			try
			{
				var request = WebRequest.Create(_url);
				var response = (HttpWebResponse)request.GetResponse();
				using (var s = response.GetResponseStream())
				{
					// doubt that GetResponseStream ever returns null, but just in case
					if(s == null)
					{
						_error = true;
						_errorMessage = "No response stream available.";
						return;
					}

					using (var reader = new StreamReader(s, GetEncoding(response.CharacterSet)))
					{
						var template = new ActiveTemplate(reader);
						var html = template.Evaluate(_data);
						writer.Write(html);
					}
				}
			}
			catch (WebException e)
			{
				// explicitly handle 404 to provide a helpful error message
				if(e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
				{
					SetError(string.Format("The template {0} was not found (404).", _url));
				}
				else
				{
					SetError(e.Message);
				}
				throw;
			}
			catch (Exception e)
			{
				SetError(e.Message);
				throw;
			}
		}

		private Encoding GetEncoding(string enc)
		{
			try
			{
				return Encoding.GetEncoding(enc);
			} 
			catch(Exception)
			{
				return Encoding.Default;
			}
		}

		private void RunConverter(string outputFilePath)
		{
			var settings = new PrintSettings();
			try
			{
				var sourcePath = string.Format("{0}?id={1}", _url.AbsolutePath, _id.ToString("N"));
				var sourceUrl = new Uri(new Uri(_httpServer.Host), sourcePath);

				var startInfo = new ProcessStartInfo(settings.ConverterProgram, string.Format("{0} {1} {2}", settings.ConverterOptions, sourceUrl, outputFilePath));
				startInfo.UseShellExecute = false;
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
	}
}
