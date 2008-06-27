using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class PrintReportTool : Tool<IToolContext>
	{
		public void Apply()
		{
			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				IReportingWorkflowItemToolContext context = (IReportingWorkflowItemToolContext)this.ContextBase;
				ReportingWorklistItem item = CollectionUtils.FirstElement(context.SelectedItems);

				PrintReportComponent component = new PrintReportComponent(
					item.PatientProfileRef,
					item.OrderRef,
					item.ReportRef);

				ApplicationComponent.LaunchAsDialog(
					context.DesktopWindow,
					component,
					SR.TitlePrintReport);
			}
		}
	}
}
