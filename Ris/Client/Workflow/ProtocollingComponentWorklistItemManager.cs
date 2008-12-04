using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItem, IReportingWorkflowService>
	{
		public ProtocollingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItem worklistItem)
		{
			throw new NotSupportedException("Protocolling component mode should be initialized externally.  ReportingWorklistItem does not have enough context.");
		}

		protected override string TaskName
		{
			get { return "Protocolling"; }
		}
	}

	public static class ProtocollingComponentModes
	{
		public static AssignProtocolMode Assign = new AssignProtocolMode();
		public static EditProtocolMode Edit = new EditProtocolMode();
		public static ReviewProtocolMode Review = new ReviewProtocolMode();
	}

	public class AssignProtocolMode : ContinuousWorkflowComponentMode
	{
		public AssignProtocolMode()
			: base(true, true, true)
		{
		}
	}

	public class EditProtocolMode : ContinuousWorkflowComponentMode
	{
		public EditProtocolMode()
			: base(false, false, false)
		{
		}
	}

	public class ReviewProtocolMode : ContinuousWorkflowComponentMode
	{
		public ReviewProtocolMode()
			: base(false, false, false)
		{
		}
	}
}