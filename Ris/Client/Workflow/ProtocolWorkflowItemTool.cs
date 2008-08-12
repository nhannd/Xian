using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class ProtocolWorkflowItemTool : WorkflowItemTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
		protected ProtocolWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected ReportingWorklistItem GetSelectedItem()
		{
			if (this.Context.SelectedItems.Count != 1)
				return null;
			return CollectionUtils.FirstElement(this.Context.SelectedItems);
		}
	}
}
