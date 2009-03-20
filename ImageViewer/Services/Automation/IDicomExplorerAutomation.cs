using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.Automation;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IDicomExplorerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IDicomExplorerAutomation
	{
		[FaultContract(typeof(NoLocalStoreFault))]
		[FaultContract(typeof(DicomExplorerNotFoundFault))]
		[OperationContract(IsOneWay = false)]
		SearchLocalStudiesResult SearchLocalStudies(SearchLocalStudiesRequest request);

		[FaultContract(typeof(ServerNotFoundFault))]
		[FaultContract(typeof(DicomExplorerNotFoundFault))]
		[OperationContract(IsOneWay = false)]
		SearchRemoteStudiesResult SearchRemoteStudies(SearchRemoteStudiesRequest request);
	}
}
