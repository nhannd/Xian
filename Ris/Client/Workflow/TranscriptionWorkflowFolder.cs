using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class TranscriptionWorkflowFolder : WorklistFolder<ReportingWorklistItem, ITranscriptionWorkflowService>
	{
		public TranscriptionWorkflowFolder()
			: base(new ReportingWorklistTable())
		{
		}
	}
}