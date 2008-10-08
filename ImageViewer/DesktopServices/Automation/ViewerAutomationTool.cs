using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
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

	//[ButtonAction("test", "global-menus/Test/Test Automation Client", "TestClient")]
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

		private void TestClient()
		{
			SynchronizationContext context = SynchronizationContext.Current;

			//Have to test client on another thread, otherwise there is a deadlock b/c the service is hosted on the main thread.
			ThreadPool.QueueUserWorkItem(delegate
			                                {
												try
												{
													using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
													{
														client.GetActiveViewerSessions();
													}

													context.Post(delegate
											             			{
											             				base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
											             			}, null);
												}
												catch (Exception e)
												{
													context.Post(delegate
													{
														base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
													}, null);

												}
											});

		}
	}
}
