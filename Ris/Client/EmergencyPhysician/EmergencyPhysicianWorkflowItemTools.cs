using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionOf(typeof(EmergencyPhysicianMainWorkflowItemToolExtensionPoint))]
	public class EmergencyPhysicianEmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string Title
		{
			get { return this.SummaryItem.AccessionNumber; }
		}
	}

	[ExtensionOf(typeof(EmergencyPhysicianOrderNoteboxItemToolExtensionPoint))]
	public class EmergencyPhysicianOrderNoteboxConversationTool : PreliminaryDiagnosisConversationTool<OrderNoteboxItemSummary, IOrderNoteboxItemToolContext>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string Title
		{
			get { return this.SummaryItem.AccessionNumber; }
		}
	}
}
