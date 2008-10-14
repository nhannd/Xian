using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.Automation;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IViewerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IViewerAutomation
	{
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(NoActiveViewersFault))]
		GetActiveViewersResult GetActiveViewers();

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(OpenStudiesFault))]
		OpenStudiesResult OpenStudies(OpenStudiesRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void ActivateViewer(ActivateViewerRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void CloseViewer(CloseViewerRequest request);
	}
}