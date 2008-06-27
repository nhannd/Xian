using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Workflow.Folders;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReportingMainWorkflowConversationTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
	}
}