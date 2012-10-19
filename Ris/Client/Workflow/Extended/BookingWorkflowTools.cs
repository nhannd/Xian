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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingReplaceOrderTool : ReplaceOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.ToBeScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.PendingProtocolFolder));
		}
	}

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.ToBeScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.PendingProtocolFolder));
		}
	}

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingModifyOrderTool : ModifyOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
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
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.ToBeScheduledFolder));
		}
	}

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingUnmergeOrdersTool : UnmergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.ToBeScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.PendingProtocolFolder));
		}
	}

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingMergeOrdersTool : MergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.ToBeScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Booking.PendingProtocolFolder));
		}
	}

}
