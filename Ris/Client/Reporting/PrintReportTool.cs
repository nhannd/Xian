using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
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
				ApplicationComponent.LaunchAsDialog(
					context.DesktopWindow, 
					new DialogBoxCreationArgs(new PrintReportComponent(item), "Foo", "Foo", DialogSizeHint.Large));
			}
		}
	}

	public class PrintReportComponent : DHtmlComponent
	{
		private readonly ReportingWorklistItem _item;

		public PrintReportComponent(ReportingWorklistItem owner)
		{
			_item = owner;
		}

		public override void Start()
		{
			SetUrl(this.PageUrl);
			base.Start();
		}

		protected virtual string PageUrl
		{
			get { return WebResourcesSettings.Default.PrintReportPreviewUrl; }
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _item;
		}

	}
}
