using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Automation
{
	[ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = "http://www.clearcanvas.ca/imageViewer/automation/contracts")]
	public interface IViewerAutomation
	{
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(NoActiveViewerSessionsFault))]
		GetActiveViewerSessionsResult GetActiveViewerSessions();

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerSessionNotFoundFault))]
		GetViewerSessionInfoResult GetViewerSessionInfo(GetViewerSessionInfoRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(OpenStudiesFault))]
		OpenStudiesResult OpenStudies(OpenStudiesRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerSessionNotFoundFault))]
		void ActivateViewerSession(ActivateViewerSessionRequest request);

		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerSessionNotFoundFault))]
		void CloseViewerSession(CloseViewerSessionRequest request);
	}
}
