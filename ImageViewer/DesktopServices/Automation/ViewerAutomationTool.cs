using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.Automation;
using System.Threading;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
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
	public class ViewerAutomationTool : DesktopHostTool
	{
		internal static SynchronizationContext HostSynchronizationContext;

		public ViewerAutomationTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			HostSynchronizationContext = SynchronizationContext.Current;

			ServiceHost host = new ServiceHost(typeof(ViewerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}
	}
}
