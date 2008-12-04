using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionDocument : Document
	{
		private readonly ReportingWorklistItem _worklistItem;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;

		public TranscriptionDocument(ReportingWorklistItem worklistItem, IReportingWorkflowItemToolContext context)
			: base(worklistItem.ProcedureStepRef, context.DesktopWindow)
		{
			_worklistItem = worklistItem;
			_folderName = context.SelectedFolder.Name;

			if(context.SelectedFolder is TranscriptionWorkflowFolder)
			{
				_worklistRef = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		public override string GetTitle()
		{
			return TranscriptionDocument.GetTitle(_worklistItem);
		}

		public override IApplicationComponent GetComponent()
		{
			return new TranscriptionComponent(_worklistItem, _folderName, _worklistRef, _worklistClassName);
		}

		public static string GetTitle(ReportingWorklistItem item)
		{
			return string.Format("Transcription - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}
	}
}