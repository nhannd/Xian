using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Reassign", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reassign", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Reassign)]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Reassign)]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class ReassignTool : ReportingWorkflowItemTool
	{
		public ReassignTool()
			: base("ReassignProcedureStep")
		{
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			try
			{
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					new ReassignComponent(item),
					SR.TitleReassignItem);

				return exitCode == ApplicationComponentExitCode.Accepted;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return false;
			}
		}
	}
}
