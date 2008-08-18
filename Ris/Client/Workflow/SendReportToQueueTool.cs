using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print//Fax Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print//Fax Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class SendReportToQueueTool : ReportingWorkflowItemTool
	{
		public SendReportToQueueTool()
			: base("SendReportToQueue")
		{
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1 || !this.Context.GetOperationEnablement("SendReportToQueue"))
					return false;

				ReportingWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				if (item.ReportRef == null && item.ProcedureRef == null)
					return false;

				return true;
			}
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			try
			{
				PublishReportComponent component = new PublishReportComponent(
						item.PatientProfileRef,
						item.OrderRef,
						item.ProcedureRef,
						item.ReportRef);

				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintReport);

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
