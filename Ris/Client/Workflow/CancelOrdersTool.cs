#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Order", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Order", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CancelOrderSmall.png", "Icons.CancelOrderMedium.png", "Icons.CancelOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Cancel)]
	public abstract class CancelOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		protected CancelOrderToolBase()
			: base("CancelOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// bug #3199: cancel operation moved to IOrderEntryService -> need to register for enablement
			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			if (OrderCancelHelper.CancelOrder(item.OrderRef, item.PatientName, this.Context.DesktopWindow))
			{
				InvalidateFolders();
				return true;
			}
			return false;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CancelledFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.CancelledFolder));
		}
	}


	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingCancelOrderTool : CancelOrderToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
	{
		protected override bool Execute(ModalityWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledFolder));
		}
	}
}