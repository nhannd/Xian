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
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using TimeoutException=System.ServiceProcess.TimeoutException;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("start", "global-menus/MenuTools/MenuServices/MenuStart", "StartService")]
	[EnabledStateObserver("start", "StartEnabled", "EnabledChanged")]
	[IconSet("start", IconScheme.Colour, "Icons.StartServiceToolSmall.png", "Icons.StartServiceToolMedium.png", "Icons.StartServiceToolLarge.png")]
	[ViewerActionPermission("start", AuthorityTokens.Management.Services)]

	[MenuAction("stop", "global-menus/MenuTools/MenuServices/MenuStop", "StopService")]
	[EnabledStateObserver("stop", "StopEnabled", "EnabledChanged")]
	[IconSet("stop", IconScheme.Colour, "Icons.StopServiceToolSmall.png", "Icons.StopServiceToolMedium.png", "Icons.StopServiceToolLarge.png")]
	[ViewerActionPermission("stop", AuthorityTokens.Management.Services)]

	[MenuAction("restart", "global-menus/MenuTools/MenuServices/MenuRestart", "RestartService")]
	[EnabledStateObserver("restart", "StopEnabled", "EnabledChanged")]
	[IconSet("restart", IconScheme.Colour, "Icons.RestartServiceToolSmall.png", "Icons.RestartServiceToolMedium.png", "Icons.RestartServiceToolLarge.png")]
	[ViewerActionPermission("restart", AuthorityTokens.Management.Services)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ServiceControlTool : Tool<IDesktopToolContext>
	{
		private Timer _timer;
		private bool _startEnabled;
		private bool _stopEnabled;

		public ServiceControlTool()
		{
		}

		public bool StartEnabled
		{
			get { return _startEnabled; }
			set
			{
				if (_startEnabled == value)
					return;

				_startEnabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

		public bool StopEnabled
		{
			get { return _stopEnabled; }
			set
			{
				if (_stopEnabled == value)
					return;

				_stopEnabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			_timer = new Timer(OnTimer);
			_timer.IntervalMilliseconds = 10000;
			_timer.Start();

			UpdateEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (_timer != null)
			{
				_timer.Stop();
				_timer = null;
			}

			base.Dispose(disposing);
		}

		private void OnTimer(object nothing)
		{
			if (_timer != null)
				UpdateEnabled();
		}

		private static ServiceController CreateServiceController()
		{
			return new ServiceController(ServiceControlSettings.Default.ServiceName);
		}

		private void UpdateEnabled()
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					StartEnabled = controller.Status == ServiceControllerStatus.Stopped;
					StopEnabled = controller.Status == ServiceControllerStatus.Running;
				}
			}
			catch(Exception)
			{
				Platform.Log(LogLevel.Debug, "Failed to determine state of service '{0}'.", ServiceControlSettings.Default.ServiceName);
				StartEnabled = false;
				StopEnabled = false;
			}
		}

		public void StopService()
		{
			BlockingOperation.Run(delegate { StopServiceInternal(); });
		}

		public void StartService()
		{
			BlockingOperation.Run(delegate { StartServiceInternal(); });
		}

		public void RestartService()
		{
			BlockingOperation.Run(delegate { RestartServiceInternal(); });
		}

		private void RestartServiceInternal()
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					if (controller.Status == ServiceControllerStatus.Running)
					{
						controller.Stop();
						controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(ServiceControlSettings.Default.TimeoutSeconds));
						if (controller.Status != ServiceControllerStatus.Stopped)
						{
							throw new TimeoutException();
						}
					}

					if (controller.Status == ServiceControllerStatus.Stopped)
					{
						controller.Start();
						controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ServiceControlSettings.Default.TimeoutSeconds));
						if (controller.Status != ServiceControllerStatus.Running)
						{
							throw new TimeoutException();
						}
					}
					else
					{
						throw new InvalidOperationException("The service is not in the correct state to be restarted.");
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e, "Failed to restart service '{0}'.", ServiceControlSettings.Default.ServiceName);
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageFailedToRestartService, MessageBoxActions.Ok);
			}
			
			UpdateEnabled();
		}

		private void StopServiceInternal()
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					controller.Stop();
					controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(ServiceControlSettings.Default.TimeoutSeconds));
					if (controller.Status != ServiceControllerStatus.Stopped)
					{
						throw new TimeoutException();
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e, "Failed to stop service '{0}'.", ServiceControlSettings.Default.ServiceName);
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageFailedToStopService, MessageBoxActions.Ok);
			}

			UpdateEnabled();
		}

		private void StartServiceInternal()
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					controller.Start();
					controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ServiceControlSettings.Default.TimeoutSeconds));
					if (controller.Status != ServiceControllerStatus.Running)
					{
						throw new TimeoutException();
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e, "Failed to start service '{0}'.", ServiceControlSettings.Default.ServiceName);
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageFailedToStartService, MessageBoxActions.Ok);
			}

			UpdateEnabled();
		}
	}
}
