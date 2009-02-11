using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
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

		protected EntityRef GetSupervisorRef()
		{
			ProtocollingSupervisorSelectionComponent component = new ProtocollingSupervisorSelectionComponent();
			if (ApplicationComponentExitCode.Accepted == ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleSelectSupervisor))
			{
				return component.Staff != null ? component.Staff.StaffRef : null;
			}
			else
				return null;
		}
	}
}
