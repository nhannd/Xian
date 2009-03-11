using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	//NOTE: DEBUG'd out because there is no requirement for this feature, however it is a useful debugging tool (JR)
#if DEBUG
	[ButtonAction("view", "folderexplorer-items-toolbar/Workflow History", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Workflow History", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Workflow History", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Workflow History", "View")]
#endif
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "View workflow history")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class WorkflowHistoryTool : Tool<IWorkflowItemToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectionChanged += delegate
			{
				this.Enabled = DetermineEnablement();
			};
		}

		private bool DetermineEnablement()
		{
			return this.Context.Selection != null && this.Context.Selection.Items.Length == 1;
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

		public void View()
		{
			WorklistItemSummaryBase item = (WorklistItemSummaryBase) this.Context.Selection.Item;
			Open(item.OrderRef, this.Context.DesktopWindow);
		}

		protected static void Open(EntityRef orderRef, IDesktopWindow window)
		{
			ApplicationComponent.LaunchAsDialog(
				window,
				new WorkflowHistoryComponent(orderRef),
				"Workflow History");
		}
	}
}
