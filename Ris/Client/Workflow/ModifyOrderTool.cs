#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Modify Order", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Modify Order", "Apply")]
	[IconSet("apply", IconScheme.Colour, "ModifyOrderSmall.png", "ModifyOrderMedium.png", "ModifyOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Order.Modify)]
	public abstract class ModifyOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		public ModifyOrderToolBase()
			: base("ModifyOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			OrderEditorComponent component = new OrderEditorComponent(
				item.PatientRef,
				item.PatientProfileRef,
				item.OrderRef,
				OrderEditorComponent.Mode.ModifyOrder);

			IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				component,
				string.Format("Modify Order - {0} {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)));

			Type selectedFolderType = this.Context.SelectedFolder.GetType();
			workspace.Closed += delegate
				{
					if (component.ExitCode == ApplicationComponentExitCode.Accepted)
					{
						InvalidateFolders();
						DocumentManager.InvalidateFolder(selectedFolderType);
					}
				};

			// Return false because even though the modify order component is opened, no workflow action has actually taken place.
			return false;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationModifyOrderTool : ModifyOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(
					this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.CheckedInFolder));
		}
	}

	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingModifyOrderTool : ModifyOrderToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
	{
		protected override bool Execute(ModalityWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.CheckedInFolder));
		}
	}
}
