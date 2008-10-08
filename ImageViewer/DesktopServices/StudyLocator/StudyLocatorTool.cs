using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.StudyLocator;
using System.Threading;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class StudyLocatorTool : DesktopHostTool
	{
		internal static SynchronizationContext HostSynchronizationContext;

		public StudyLocatorTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			HostSynchronizationContext = SynchronizationContext.Current;

			ServiceHost host = new ServiceHost(typeof(ClearCanvas.ImageViewer.StudyLocator.StudyLocator));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = StudyLocatorNamespace.Value;

			return host;
		}
	}
}