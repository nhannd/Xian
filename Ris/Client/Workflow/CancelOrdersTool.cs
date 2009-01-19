using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Order", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Order", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CancelOrderSmall.png", "Icons.CancelOrderMedium.png", "Icons.CancelOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Order.Cancel)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class CancelOrdersTool : RegistrationWorkflowTool
	{
		public CancelOrdersTool()
			: base("CancelOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

            // bug #3199: cancel operation moved to IOrderEntryService -> need to register for enablement
            this.Context.RegisterWorkflowService(typeof(IOrderEntryService));

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CancelledFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItem item)
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

				this.Context.InvalidateFolders(typeof(Folders.Registration.ScheduledFolder));
				this.Context.InvalidateFolders(typeof(Folders.Registration.CancelledFolder));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}