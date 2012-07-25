#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Cancel and Replace Order", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Cancel and Replace Order", "Apply")]
	[IconSet("apply", IconScheme.Colour, "ReplaceOrderSmall.png", "ReplaceOrderMedium.png", "ReplaceOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Replace)]
	public abstract class ReplaceOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>

	{
		protected ReplaceOrderToolBase()
			: base("ReplaceOrder")
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
			// first check for warnings
			var warnings = new List<string>();
			Platform.GetService<IOrderEntryService>(
				service => warnings = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(item.OrderRef)).Warnings);

			if(warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(warnings);
				var action = this.Context.DesktopWindow.ShowMessageBox(
					warn + "\n\nAre you sure you want to cancel and replace this order?",
					MessageBoxActions.YesNo);
				if(action == DialogBoxAction.No)
					return false;
			}

			var component = new OrderEditorComponent(item.OrderRef, OrderEditorComponent.Mode.ReplaceOrder);
			var result = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				component,
				string.Format(SR.TitleReplaceOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)));

			if(result == ApplicationComponentExitCode.Accepted)
			{
				InvalidateFolders();
				return true;
			}

			return false;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationReplaceOrderTool : ReplaceOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
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
	public class PerformingReplaceOrderTool : ReplaceOrderToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
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
