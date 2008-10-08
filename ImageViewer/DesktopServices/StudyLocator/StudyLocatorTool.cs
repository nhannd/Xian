using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.StudyLocator;
using System.Threading;
using ClearCanvas.Desktop.Actions;
using System;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	//[ButtonAction("test", "global-menus/Test/Test Study Locator Client", "TestClient")]
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

		private void TestClient()
		{
			try
			{
				using (StudyLocatorServiceClient client = new StudyLocatorServiceClient())
				{
					client.FindByAccessionNumber("test");
				}

				base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
			}
			catch(Exception e)
			{
				base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
		}
	}
}