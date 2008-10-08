using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.Automation;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IViewerAutomation", Namespace = AutomationNamespace.Value)]
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