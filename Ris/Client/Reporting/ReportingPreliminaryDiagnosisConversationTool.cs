using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
	[ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
	public class ReportingMainWorkflowConversationTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
	}

	[ExtensionOf(typeof(ReportingOrderNoteboxItemToolExtensionPoint))]
	public class ReportingOrderNoteboxConversationTool : OrderNoteConversationTool
	{
	}
}