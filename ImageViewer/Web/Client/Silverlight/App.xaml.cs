#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls;
using ClearCanvas.Web.Client.Silverlight;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
	public partial class App : System.Windows.Application
	{
		private readonly object _startLock = new object();
		private ApplicationContext _context;

		public App()
		{

			Startup += Application_Startup;
			Exit += Application_Exit;
			UnhandledException += Application_UnhandledException;
           
            InitializeComponent();

            MenuManager.SuppressBrowserContextMenu = true;
            MenuManager.AutoCloseMenus = false;
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//TODO (CR May 2010): need the lock?
			lock (_startLock)
			{
				if (_context != null)
					return;

				// Initialize the communication channel with the host
				ApplicationBridge.Initialize();
				_context = ApplicationContext.Initialize();
			}


            Panel rootPanel = new Grid();
            RootVisual = rootPanel;

            DialogControl.ApplicationRootVisual = rootPanel;

			string query = HtmlPage.Document.DocumentUri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                StartupArguments args = new StartupArguments(e.InitParams);

                ServerConfiguration.Current = new ServerConfiguration
                                                   {
                                                       InactivityTimeout = args.InactivityTimeout,
                                                       LANMode = args.LANMode,
                                                       Port = args.Port
                                                   };

                Logger.SetWriteMethod(LogMessage);
                Logger.SetErrorMethod(OnError);

                StartViewerApplicationRequest request = new StartViewerApplicationRequest
                {
                    AccessionNumber = new ObservableCollection<string>(),
                    StudyInstanceUid = new ObservableCollection<string>(),
                    PatientId = new ObservableCollection<string>()
                };
                
                string[] vals = HttpUtility.UrlDecode(query).Split(new[] { '?', ';', '=', ',', '&' });
                for (int i = 0; i < vals.Length - 1; i++)
                {
                    if (String.IsNullOrEmpty(vals[i]))
                        continue;

                    if (vals[i].Equals("study"))
                    {
                        i++;
                        request.StudyInstanceUid.Add(vals[i]);
                    }
                    else if (vals[i].Equals("patientid"))
                    {
                        i++;
                        request.PatientId.Add(vals[i]);
                    }
                    else if (vals[i].Equals("aetitle"))
                    {
                        i++;
                        request.AeTitle = vals[i];
                    }
                    else if (vals[i].Equals("accession"))
                    {
                        i++;
                        request.AccessionNumber.Add(vals[i]);
                    }
                    else if (vals[i].Equals("application"))
                    {
                        i++;
                        request.ApplicationName = vals[i];
                    }
                }

                request.Username = ApplicationContext.Current.Parameters.Username;
                request.SessionId = ApplicationContext.Current.Parameters.SessionToken;
                request.IsSessionShared = ApplicationContext.Current.Parameters.IsSessionShared;

                rootPanel.Children.Add(new ImageViewer(request));
            }
            else
            {  
                rootPanel.Children.Add(new ImageViewer(null));
            }
		}

	    private void OnError(string msg)
        {
            if (SynchronizationContext.Current != null)
            {
                string message = msg;
                DialogControl.Show("Error", message, "Dismiss");
            }
        }

        private void LogMessage(string msg)
        {
        }

		private void Application_Exit(object sender, EventArgs e)
		{
			if (RootVisual is IDisposable)
			{
				((IDisposable)RootVisual).Dispose();
			}

			if (_context != null)
			{
				_context.Dispose();
				_context = null;
			}
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!System.Diagnostics.Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}
		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}