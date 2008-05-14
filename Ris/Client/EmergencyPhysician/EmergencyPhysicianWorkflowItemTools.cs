using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(EmergencyPhysicianMainWorkflowItemToolExtensionPoint))]
	public class EmergencyPhysicianEmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
	}

	[ExtensionOf(typeof(EmergencyPhysicianOrderNoteboxItemToolExtensionPoint))]
	public class EmergencyPhysicianOrderNoteboxConversationTool : OrderNoteConversationTool
	{
	}
}
