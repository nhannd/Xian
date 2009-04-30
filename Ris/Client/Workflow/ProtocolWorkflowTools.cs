#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Verify Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Verify Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class VerifyProtocolTool : ProtocolWorkflowItemTool
	{
		public VerifyProtocolTool()
			: base("AcceptProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.CompletedProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			try
			{
				ExecuteHelper(item.ProcedureStepRef, null);
			}
			catch (FaultException<SupervisorValidationException>)
			{
				ExecuteHelper(item.ProcedureStepRef, GetSupervisorRef());
			}

			this.Context.InvalidateFolders(typeof(Folders.Reporting.CompletedProtocolFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					AcceptProtocolRequest request = new AcceptProtocolRequest(procedureStepRef, supervisorRef);
					service.AcceptProtocol(request);
				});
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
			: base("RejectProtocol")
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
				try
				{
					ExecuteHelper(item.ProcedureStepRef, component.Reason, component.OtherReason, null);
				}
				catch (FaultException<SupervisorValidationException>)
				{
					ExecuteHelper(item.ProcedureStepRef, component.Reason, component.OtherReason, GetSupervisorRef());
				}

				this.Context.InvalidateFolders(typeof (Folders.Reporting.RejectedProtocolFolder));
			}
			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EnumValueInfo reason, string otherReason, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					RejectProtocolRequest request = new RejectProtocolRequest(
						procedureStepRef, 
						supervisorRef,
						reason,
						CreateAdditionalCommentsNote(otherReason));
					service.RejectProtocol(request);
				});
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
			try
			{
				ExecuteHelper(item.ProcedureStepRef, null);
			}
			catch (FaultException<SupervisorValidationException>)
			{
				ExecuteHelper(item.ProcedureStepRef, GetSupervisorRef());
			}

			this.Context.InvalidateFolders(typeof(Folders.Reporting.AwaitingApprovalProtocolFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					SubmitProtocolForApprovalRequest request = new SubmitProtocolForApprovalRequest(procedureStepRef, supervisorRef);
					service.SubmitProtocolForApproval(request);
				});
		}
	}

    [MenuAction("apply", "folderexplorer-items-contextmenu/Discard Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Discard Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class DiscardProtocolTool : ProtocolWorkflowItemTool
	{
		public DiscardProtocolTool()
			: base("DiscardProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.ToBeProtocolledFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDiscardSelectedProtocol, MessageBoxActions.OkCancel)
				== DialogBoxAction.Cancel)
				return false;

			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					service.DiscardProtocol(new DiscardProtocolRequest(item.ProcedureStepRef));
				});

			return true;
		}
	}
}