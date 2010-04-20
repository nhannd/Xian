using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/Merge Orders", "Apply")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/Merge Orders", "Apply")]
	[Tooltip("apply", "Merge Orders")]
	[IconSet("apply", IconScheme.Colour, "ReplaceOrderSmall.png", "ReplaceOrderMedium.png", "ReplaceOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Merge)]
	public class MergeOrdersTool : WorkflowItemTool<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		public MergeOrdersTool()
			: base("MergeOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 2)
					return false;

				var list = new List<RegistrationWorklistItemSummary>(this.Context.SelectedItems);

				// Obvious cases where merging should not be allowed.
				// Cannot merge the same order.
				if (list[0].AccessionNumber == list[1].AccessionNumber)
					return false;

				// Cannot merge orders from different patient
				if (!list[0].PatientRef.Equals(list[1].PatientRef, true))
					return false;

				// Return true, let the server decide how to inform user of more complicated error.
				return true;
			}
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			var list = new List<RegistrationWorklistItemSummary>(this.Context.SelectedItems);
			var component = new MergeOrdersComponent(list[0].OrderRef, list[1].OrderRef);

			string failureReason;
			if (!component.ValidateMergeRequest(out failureReason))
			{
				this.Context.DesktopWindow.ShowMessageBox(failureReason, MessageBoxActions.Ok);
				return false;
			}

			var result = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleMergeOrders);
			return result == ApplicationComponentExitCode.Accepted;
		}
	}
}
