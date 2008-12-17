using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.Automation;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// Service contract for automation of the viewer.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IViewerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IViewerAutomation
	{
		/// <summary>
		/// Gets all active <see cref="Viewer"/>s.
		/// </summary>
		/// <exception cref="FaultException{NoActiveViewersFault}">Thrown if there are no active <see cref="Viewer"/>s.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(NoActiveViewersFault))]
		GetActiveViewersResult GetActiveViewers();

		/// <summary>
		/// Gets information about the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request);

		/// <summary>
		/// Opens the requested studies in a <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{OpenStudiesFault}">Thrown if the primary study could not be opened.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(OpenStudiesFault))]
		OpenStudiesResult OpenStudies(OpenStudiesRequest request);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void ActivateViewer(ActivateViewerRequest request);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void CloseViewer(CloseViewerRequest request);
	}
}