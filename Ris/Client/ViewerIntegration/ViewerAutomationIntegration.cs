using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.Automation;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.Ris.Client.ViewerIntegration
{
	[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
	public class ViewerAutomationIntegration : IViewerIntegration
	{
		public ViewerAutomationIntegration()
		{
		}

		private static IViewerAutomationBridge CreateBridge()
		{
			return new ViewerAutomationBridge(
				Platform.GetService<IViewerAutomation>(), 
				Platform.GetService<IStudyRootQuery>());
		}

		#region IViewerIntegration Members

		public void Open(string accessionNumber)
		{
			using (IViewerAutomationBridge bridge = CreateBridge())
				bridge.OpenStudiesByAccessionNumber(accessionNumber);
		}

		public void Close(string accessionNumber)
		{
			using (IViewerAutomationBridge bridge = CreateBridge())
			{
				foreach (Viewer viewer in bridge.GetViewersByAccessionNumber(accessionNumber))
					bridge.CloseViewer(viewer);
			}
		}

		public void Activate(string accessionNumber)
		{
			using (IViewerAutomationBridge bridge = CreateBridge())
			{
				foreach (Viewer viewer in bridge.GetViewersByAccessionNumber(accessionNumber))
				{
					bridge.ActivateViewer(viewer);
					return;
				}
			}
		}

		#endregion
	}
}
