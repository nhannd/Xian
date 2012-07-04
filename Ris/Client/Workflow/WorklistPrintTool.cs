#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
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
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Worklist.Print)]
	public class WorklistPrintTool : Tool<IWorkflowItemToolContext>
	{
		public bool Enabled
		{
			get { return this.Context.SelectedFolder != null && this.Context.SelectedFolder.ItemsTable.Items.Count > 0; }
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
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
				SR.TitlePrintWorklist);
		}
	}
}
