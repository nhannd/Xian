using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReportingPreliminaryDiagnosisTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
		protected override string InitialNoteText
		{
			get
			{
				return PreliminaryDiagnosisSettings.Default.DefaultReviewText;
			}
		}
	}
}