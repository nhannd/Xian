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
				return this.Context.SelectedItems.Count == 2;
			}
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			var list = new List<RegistrationWorklistItemSummary>(this.Context.SelectedItems);
			var order1Ref = list[0].OrderRef;
			var order2Ref = list[1].OrderRef;
			var result = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				new MergeOrdersComponent(order1Ref, order2Ref),
				SR.TitleMergeOrders);

			return result == ApplicationComponentExitCode.Accepted;
		}
	}
}
