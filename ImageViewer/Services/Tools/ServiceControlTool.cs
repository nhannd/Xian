#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using TimeoutException=System.ServiceProcess.TimeoutException;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("start", "global-menus/MenuTools/MenuServices/MenuStart", "StartService")]
	[EnabledStateObserver("start", "StartEnabled", "EnabledChanged")]
	[IconSet("start", "Icons.StartServiceToolSmall.png", "Icons.StartServiceToolMedium.png", "Icons.StartServiceToolLarge.png")]
	[ViewerActionPermission("start", AuthorityTokens.Administration.Services)]

	[MenuAction("stop", "global-menus/MenuTools/MenuServices/MenuStop", "StopService")]
	[EnabledStateObserver("stop", "StopEnabled", "EnabledChanged")]
	[IconSet("stop", "Icons.StopServiceToolSmall.png", "Icons.StopServiceToolMedium.png", "Icons.StopServiceToolLarge.png")]
	[ViewerActionPermission("stop", AuthorityTokens.Administration.Services)]

	[MenuAction("restart", "global-menus/MenuTools/MenuServices/MenuRestart", "RestartService")]
	[EnabledStateObserver("restart", "StopEnabled", "EnabledChanged")]
	[IconSet("restart", "Icons.RestartServiceToolSmall.png", "Icons.RestartServiceToolMedium.png", "Icons.RestartServiceToolLarge.png")]
	[ViewerActionPermission("restart", AuthorityTokens.Administration.Services)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ServiceControlTool : Tool<IDesktopToolContext>
	{
		private enum ServiceControlOperation
		{
			Start,
			Stop,
			Restart
		}

		private static readonly string _serviceName = ServiceControlSettings.Default.ServiceName;
		private static readonly int _timeoutSeconds = ServiceControlSettings.Default.TimeoutSeconds;

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
			Refresh();
		}

		protected override void Dispose(bool disposing)
		{
			StopTimer();
			base.Dispose(disposing);
		}

		#region Refresh

		private void StartTimer()
		{
			if (_timer != null)
				return;

			_timer = new Timer(OnTimer) { IntervalMilliseconds = 10000 };
			_timer.Start();
		}

		private void StopTimer()
		{
			if (_timer == null)
				return;
			
			_timer.Stop();
			_timer = null;
		}

		private void OnTimer(object nothing)
		{
			if (_timer != null)
				Refresh();
		}

		private void OnUnknownService()
		{
			Platform.Log(LogLevel.Debug, "Failed to determine state of service '{0}' because it doesn't exist.", _serviceName);
			
			StopTimer();
			StartEnabled = true; //Leave one button enabled so the user can try to refresh status.
			StopEnabled = false;
		}

		private void OnUnknownError(Exception e)
		{
			Platform.Log(LogLevel.Debug, e, "Failed to determine state of service '{0}'.", _serviceName);

			StopTimer();
			StartEnabled = true; //Leave one button enabled so the user can try to refresh status.
			StopEnabled = false;
		}

		private void Refresh()
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					StartEnabled = controller.Status == ServiceControllerStatus.Stopped;
					StopEnabled = controller.Status == ServiceControllerStatus.Running;
				}

				StartTimer();
			}
			catch (InvalidOperationException)
			{
				OnUnknownService();
			}
			catch (Exception e)
			{
				OnUnknownError(e);
			}
		}

		#endregion

		private static ServiceController CreateServiceController()
		{
			return new ServiceController(_serviceName);
		}

		#region Service Control

		public void StopService()
		{
			BlockingOperation.Run(() => ControlService(StopService, ServiceControlOperation.Stop));
		}

		public void StartService()
		{
			BlockingOperation.Run(() => ControlService(StartService, ServiceControlOperation.Start));
		}

		public void RestartService()
		{
			BlockingOperation.Run(() => ControlService(RestartService, ServiceControlOperation.Restart));
		}

		private void ControlService(Action<ServiceController> controlMethod, ServiceControlOperation operation)
		{
			try
			{
				using (ServiceController controller = CreateServiceController())
				{
					controlMethod(controller);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e, "Failed to {0} service '{1}'.", operation.ToString().ToLower(), _serviceName);

				if (IsAccessDeniedError(e))
					Context.DesktopWindow.ShowMessageBox(SR.MessageControlServiceAccessDenied, MessageBoxActions.Ok);
				else if (e is TimeoutException)
					Context.DesktopWindow.ShowMessageBox(SR.MessageControlServiceTimeout, MessageBoxActions.Ok);
				else
					Context.DesktopWindow.ShowMessageBox(SR.MessageFailedToStartService, MessageBoxActions.Ok);
			}

			Refresh();
		}

		private static void RestartService(ServiceController controller)
		{
			if (controller.Status == ServiceControllerStatus.Running)
			{
				controller.Stop();
				controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(_timeoutSeconds));
				if (controller.Status != ServiceControllerStatus.Stopped)
					throw new TimeoutException();
			}

			if (controller.Status == ServiceControllerStatus.Stopped)
			{
				controller.Start();
				controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(_timeoutSeconds));
				if (controller.Status != ServiceControllerStatus.Running)
					throw new TimeoutException();
			}
			else
			{
				throw new InvalidOperationException("The service is not in the correct state to be restarted.");
			}
		}

		private static void StopService(ServiceController controller)
		{
			if (controller.Status != ServiceControllerStatus.Stopped)
			{
				//Note: don't remove the surrounding if because it tells us if the service name is wrong.
				controller.Stop();
				controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(_timeoutSeconds));
			}

			if (controller.Status != ServiceControllerStatus.Stopped)
				throw new TimeoutException();
		}

		private static void StartService(ServiceController controller)
		{
			if (controller.Status != ServiceControllerStatus.Running)
			{
				//Note: don't remove the surrounding if because it tells us if the service name is wrong.
				controller.Start();
				controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(_timeoutSeconds));
			}

			if (controller.Status != ServiceControllerStatus.Running)
				throw new TimeoutException();
		}

		private static bool IsAccessDeniedError(Exception e)
		{
			const int win32ErrorAccessDenied = 0x5;
			return e.InnerException != null && e.InnerException is Win32Exception && ((Win32Exception)e.InnerException).NativeErrorCode == win32ErrorAccessDenied;
		}

		#endregion
	}
}
