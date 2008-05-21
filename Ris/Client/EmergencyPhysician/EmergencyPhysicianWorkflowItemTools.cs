using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;
using ClearCanvas.Ris.Client.EmergencyPhysician.Folders;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(EmergencyPhysicianMainWorkflowItemToolExtensionPoint))]
	public class EmergencyPhysicianEmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
	}

	[ExtensionOf(typeof(EmergencyPhysicianOrderNoteboxItemToolExtensionPoint))]
	public class EmergencyPhysicianOrderNoteboxConversationTool : OrderNoteConversationTool
	{
		protected override void OnOpenCompleted()
		{
			this.Context.FolderSystem.InvalidateFolder(typeof(InboxFolder));
			this.Context.FolderSystem.InvalidateFolder(typeof(SentItemsFolder));
			base.OnOpenCompleted();
		}
	}
}
