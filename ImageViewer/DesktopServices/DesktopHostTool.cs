using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.DesktopServices
{
	public abstract class DesktopHostTool : Tool<IDesktopToolContext>
	{
		private ServiceHost _host;
		private int _count;

		internal DesktopHostTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			if (_count == 0)
				StartHost();

			_count++;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			_count--;

			if (_count == 0)
				StopHost();
		}

		private void StartHost()
		{
			try
			{
				//For maximum flexibility, the service host is entirely configuration-based.
				ServiceHost host = CreateServiceHost();
				host.Open();
				_host = host;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unable to start automation server; another instance is likely running.");
			}
		}

		protected abstract ServiceHost CreateServiceHost();

		private void StopHost()
		{
			try
			{
				if (_host != null)
				{
					_host.Close();
					_host = null;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to stop automation server.");
			}
		}
	}
}
