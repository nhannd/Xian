using System;
using ClearCanvas.Common;
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
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Order.Cancel)]
    public abstract class CancelOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
        where TItem : WorklistItemSummaryBase
        where TContext : IWorkflowItemToolContext<TItem>
    {
        public CancelOrderToolBase()
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
            CancelOrderComponent cancelOrderComponent = new CancelOrderComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                cancelOrderComponent,
                String.Format(SR.TitleCancelOrder, PersonNameFormat.Format(item.PatientName)));

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        service.CancelOrder(new CancelOrderRequest(item.OrderRef, cancelOrderComponent.SelectedCancelReason));
                    });

                InvalidateFolders();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class RegistrationCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CancelledFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
            return ExecuteCore(item);
		}

        protected override void InvalidateFolders()
        {
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.CancelledFolder));
        }
    }

    [ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
    public class BookingCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
    {
        protected override bool Execute(RegistrationWorklistItem item)
        {
            return ExecuteCore(item);
        }

        protected override void InvalidateFolders()
        {
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.ToBeScheduledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.PendingProtocolFolder));
        }
    }

    [ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
    public class PerformingCancelOrderTool : CancelOrderToolBase<ModalityWorklistItem, IPerformingWorkflowItemToolContext>
    {
        protected override bool Execute(ModalityWorklistItem item)
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