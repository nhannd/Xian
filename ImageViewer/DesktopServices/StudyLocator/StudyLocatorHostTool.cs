using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	//[ButtonAction("test", "global-menus/Test/Test Study Locator Client", "TestClient")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class StudyLocatorHostTool : DesktopHostTool
	{
		internal static SynchronizationContext HostSynchronizationContext;

		public StudyLocatorHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			HostSynchronizationContext = SynchronizationContext.Current;

			ServiceHost host = new ServiceHost(typeof(ClearCanvas.ImageViewer.StudyLocator.StudyLocator));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = QueryNamespace.Value;

			return host;
		}

		private void TestClient()
		{
			try
			{
				using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
				{
					bridge.QueryByAccessionNumber("test");
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
