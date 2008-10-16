using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Accept Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Accept Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class AcceptProtocolTool : ProtocolWorkflowItemTool
	{
		public AcceptProtocolTool()
			: base("AcceptOrderProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.CompletedProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					//service.AcceptOrderProtocol(new AcceptOrderProtocolRequest(item.OrderRef));
					service.AcceptProtocol(new AcceptProtocolRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Reporting.CompletedProtocolFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Reject Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reject Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.RejectProtocolSmall.png", "Icons.RejectProtocolMedium.png", "Icons.RejectProtocolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class RejectProtocolTool : ProtocolWorkflowItemTool
	{
		public RejectProtocolTool()
			: base("RejectOrderProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.RejectedProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			ProtocolReasonComponent component = new ProtocolReasonComponent();
			if (this.Context.DesktopWindow.ShowDialogBox(component, "Reason") == DialogBoxAction.Ok)
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						//service.RejectOrderProtocol(new RejectOrderProtocolRequest(
						//                                item.OrderRef, 
						//                                component.Reason, 
						//                                CreateAdditionalCommentsNote(component.OtherReason)));
						service.RejectProtocol(new RejectProtocolRequest(
														item.ProcedureStepRef,
														component.Reason,
														CreateAdditionalCommentsNote(component.OtherReason)));
					});

				this.Context.InvalidateFolders(typeof (Folders.Reporting.RejectedProtocolFolder));
			}
			return true;
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			if (!string.IsNullOrEmpty(additionalComments))
				return new OrderNoteDetail(OrderNoteCategory.Protocol.Key, additionalComments, null, false, null, null);
			else
				return null;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Submit for Review", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Submit for Review", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.SubmitForReviewSmall.png", "Icons.SubmitForReviewMedium.png", "Icons.SubmitForReviewLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class SubmitForReviewProtocolTool : ProtocolWorkflowItemTool
	{
		public SubmitForReviewProtocolTool()
			: base("SubmitProtocolForApproval")
		{
		}

		public bool Visible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview);
			}
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.AwaitingApprovalProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					//service.SubmitProtocolForApproval(new SubmitProtocolForApprovalRequest(item.OrderRef));
					service.SubmitProtocolForApproval2(new SubmitProtocolForApprovalRequest2(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Reporting.AwaitingReviewFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class CancelProtocolTool : ProtocolWorkflowItemTool
	{
		public CancelProtocolTool()
			: base("DiscardOrderProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.ToBeProtocolledFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					//service.DiscardOrderProtocol(new DiscardOrderProtocolRequest(item.OrderRef, item.ProcedureStepRef));
					service.DiscardProtocol(new DiscardProtocolRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Reporting.AwaitingReviewFolder));

			return true;
		}
	}
}