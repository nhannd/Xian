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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
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
			// first check for warnings
			var warnings = new List<string>();
			Platform.GetService<IOrderEntryService>(
				service => warnings = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(item.OrderRef)).Warnings);

			if (warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(warnings);
				var action = this.Context.DesktopWindow.ShowMessageBox(
					warn + "\n\nAre you sure you want to cancel this order?",
					MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}

			var cancelOrderComponent = new CancelOrderComponent();
            var exitCode = ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                cancelOrderComponent,
                String.Format(SR.TitleCancelOrder, PersonNameFormat.Format(item.PatientName)));

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                Platform.GetService<IOrderEntryService>(
                	service => service.CancelOrder(new CancelOrderRequest(item.OrderRef, cancelOrderComponent.SelectedCancelReason)));

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