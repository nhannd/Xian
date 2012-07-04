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
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Re-submit for Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Re-submit for Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.ResubmitOrderSmall.png", "Icons.ResubmitOrderMedium.png", "Icons.ResubmitOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Resubmit)]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class ResubmitProtocolTool : WorkflowItemTool<RegistrationWorklistItemSummary, IWorkflowItemToolContext<RegistrationWorklistItemSummary>>
	{
		public ResubmitProtocolTool()
			: base("ResubmitProtocol")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Booking.RejectedProtocolFolder));
			this.Context.InvalidateFolders(typeof(Folders.Booking.PendingProtocolFolder));

			return true;
		}
	}
}
