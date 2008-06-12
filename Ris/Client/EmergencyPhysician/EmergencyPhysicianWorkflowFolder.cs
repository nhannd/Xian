using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	public abstract class EmergencyPhysicianWorkflowFolder : WorklistFolder<RegistrationWorklistItem, IRegistrationWorkflowService>
	{
		public EmergencyPhysicianWorkflowFolder(WorkflowFolderSystem folderSystem)
			: base(folderSystem, new RegistrationWorklistTable())
		{
		}
	}
}