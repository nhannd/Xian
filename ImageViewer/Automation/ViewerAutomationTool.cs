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
		private static readonly int _defaultPort = 51122;

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

		private static string GetUriBaseAddress(string customEndpoint, int? customPort)
		{
			if (!String.IsNullOrEmpty(customEndpoint))
				customEndpoint = String.Format("/{0}", customEndpoint);
			else
				customEndpoint = "";

			if (customPort == null)
				customPort = _defaultPort;

			return String.Format("http://localhost:{0}/ClearCanvas/ImageViewer/Automation{1}", customPort.Value, customEndpoint);
		}

		private void StartHost()
		{
			try
			{
				Uri baseAddress = new Uri(GetUriBaseAddress(null, null));

				BasicHttpBinding binding = new BasicHttpBinding();
				binding.Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts";

				ServiceHost host = new ServiceHost(typeof(ViewerAutomation), baseAddress);

				host.AddServiceEndpoint(typeof(IViewerAutomation), binding, "Viewer Automation Service");

				//TODO: create explicit MEX endpoint.
				ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
				if (null == metadataBehavior)
				{
					metadataBehavior = new ServiceMetadataBehavior();
					metadataBehavior.HttpGetEnabled = true;
					host.Description.Behaviors.Add(metadataBehavior);
				}

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
