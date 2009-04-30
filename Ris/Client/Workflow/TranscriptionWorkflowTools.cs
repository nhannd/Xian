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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Complete", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Complete", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class CompleteTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public CompleteTranscriptionTool()
			: base("CompleteTranscription")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Transcription.CompletedFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					service.CompleteTranscription(new CompleteTranscriptionRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Transcription.CompletedFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Reject", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reject", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.RejectProtocolSmall.png", "Icons.RejectProtocolMedium.png", "Icons.RejectProtocolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class RejectTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public RejectTranscriptionTool()
			: base("RejectTranscription")
		{
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			TranscriptionRejectReasonComponent component = new TranscriptionRejectReasonComponent();
			if (this.Context.DesktopWindow.ShowDialogBox(component, "Reason") == DialogBoxAction.Ok)
			{
				Platform.GetService<ITranscriptionWorkflowService>(
					delegate(ITranscriptionWorkflowService service)
					{
						service.RejectTranscription(new RejectTranscriptionRequest(
							item.ProcedureStepRef, 
							component.Reason,
							CreateAdditionalCommentsNote(component.OtherReason)));
					});

				this.Context.InvalidateFolders(typeof (Folders.Transcription.CompletedFolder));
			}
			return true;
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			if (!string.IsNullOrEmpty(additionalComments))
				return new OrderNoteDetail(OrderNoteCategory.General.Key, additionalComments, null, false, null, null);
			else
				return null;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Submit for Review", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Submit for Review", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class SubmitTranscriptionForReviewTool : TranscriptionWorkflowItemTool
	{
		public SubmitTranscriptionForReviewTool()
			: base("SubmitTranscriptionForReview")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Transcription.AwaitingReviewFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					service.SubmitTranscriptionForReview(new SubmitTranscriptionForReviewRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Transcription.AwaitingReviewFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Open Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Open Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class EditTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public EditTranscriptionTool()
			: base("EditTranscription")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Transcription.DraftFolder), this);
			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		public override bool Enabled
		{
			get
			{
				ReportingWorklistItem item = GetSelectedItem();

				if (this.Context.SelectedItems.Count != 1)
					return false;

				return
					this.Context.GetOperationEnablement("StartTranscription") ||

					// there is no specific workflow operation for editing a previously created draft,
					// so we enable the tool if it looks like a draft and SaveReport is enabled
					(this.Context.GetOperationEnablement("SaveTranscription") && item != null && item.ActivityStatus.Code == StepState.InProgress);
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItem> items)
		{
			// this tool is only registered as a drop handler for the Drafts folder
			// and the only operation that would make sense in this context is StartInterpretation
			return this.Context.GetOperationEnablement("StartTranscription");
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			// open the report editor
			OpenTranscriptionEditor(item);

			return true;
		}
	}
}
