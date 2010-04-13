using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print Worklist", "Print")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print Worklist", "Print")]
	[Tooltip("apply", "Print Worklist")]
	[IconSet("apply", IconScheme.Colour, "PrintSmall.png", "PrintMedium.png", "PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class WorklistPrintTool : Tool<IWorkflowItemToolContext>
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
			return this.Context.SelectedFolder != null && this.Context.SelectedFolder.ItemsTable.Items.Count > 0;
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
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Print()
		{
			var fsName = this.Context.SelectedFolder.FolderSystem.Title;
			var folderName = this.Context.SelectedFolder.Name;
			var folderDescription = this.Context.SelectedFolder.Tooltip;
			var totalItemCount = this.Context.SelectedFolder.TotalItemCount;
			var items = new List<object>();
			foreach (var item in this.Context.SelectedFolder.ItemsTable.Items)
				items.Add(item);

			ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				new WorklistPrintComponent(fsName, folderName, folderDescription, totalItemCount, items),
				"Print Worklist");
		}
	}
}
