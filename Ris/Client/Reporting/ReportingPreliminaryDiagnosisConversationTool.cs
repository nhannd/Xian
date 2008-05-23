using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Reporting.Folders;

namespace ClearCanvas.Ris.Client.Reporting
{
	[ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
	public class ReportingMainWorkflowConversationTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
	}
}