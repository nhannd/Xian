#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
	[MenuAction("apply", "folderexplorer-items-contextmenu/Undo Merge Orders", "Apply")]
	[Tooltip("apply", "Undo Merge Orders")]
	[IconSet("apply", IconScheme.Colour, "UnmergeOrdersSmall.png", "UnmergeOrdersMedium.png", "UnmergeOrdersLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Unmerge)]
	public abstract class UnmergeOrdersToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		protected UnmergeOrdersToolBase()
			: base("UnmergeOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		public override bool Enabled
		{
			get
			{
				if (DowntimeRecovery.InDowntimeRecoveryMode)
					return false;

				// we can tolerate a multi-select, as long as all selected items have the same accession number
				var accNumbers = CollectionUtils.Unique(CollectionUtils.Map(this.Context.SelectedItems, (TItem item) => item.AccessionNumber));
				if(accNumbers.Count != 1)
					return false;

				return this.Context.GetOperationEnablement("UnmergeOrder");
			}
		}

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			EnumValueInfo reason;
			var reasonCode = OrderMergeSettings.Default.UnmergeDefaultReasonCode;
			if(string.IsNullOrEmpty(reasonCode))
			{
				var cancelOrderComponent = new CancelOrderComponent();
				var exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					cancelOrderComponent,
					string.Format("Undo merge order {0}", AccessionFormat.Format(item.AccessionNumber)));

				if (exitCode != ApplicationComponentExitCode.Accepted)
					return false;

				reason = cancelOrderComponent.SelectedCancelReason;
			}
			else
			{
				// confirm
				var message = string.Format("Un-merge all orders merged into {0}?", item.AccessionNumber);
				if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
					return false;
				reason = new EnumValueInfo(reasonCode, null, null);
			}

			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					var request = new UnmergeOrderRequest(item.OrderRef) {UnmergeReason = reason};
					service.UnmergeOrder(request);
				});

			InvalidateFolders();
			return true;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationUnmergeOrdersTool : UnmergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
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
	public class PerformingUnmergeOrdersTool : UnmergeOrdersToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
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
