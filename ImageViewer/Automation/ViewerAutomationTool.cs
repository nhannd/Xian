using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using System.ServiceModel;
using System;

namespace ClearCanvas.ImageViewer.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	/// <remarks>
	/// This class is implemented as a desktop tool rather than an application tool in order
	/// to take advantage of the 'UseSynchronizationContext' WCF service behaviour, which
	/// automatically marshals all service request over to the thread on which the service host was
	/// started.
	/// </remarks>
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ViewerAutomationTool : Tool<IDesktopToolContext>
	{
		private ServiceHost _host;
		private int _count;

		public ViewerAutomationTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			
			if (_count++ == 0)
				StartHost();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (--_count == 0)
				StopHost();
		}

		private void StartHost()
		{
			try
			{
				//For maximum flexibility, the service host is entirely configuration-based.
				ServiceHost host = new ServiceHost(typeof(ViewerAutomation));
				foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
					endpoint.Binding.Namespace = AutomationNamespace.Value;

				host.Open();
				_host = host;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unable to start automation server; another instance is likely running.");
			}
		}

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
