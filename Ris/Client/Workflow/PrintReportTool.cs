using System;
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
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class PrintReportTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				((IReportingWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

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

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				return (((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems != null
					&& ((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}
			return false;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

	}
}
