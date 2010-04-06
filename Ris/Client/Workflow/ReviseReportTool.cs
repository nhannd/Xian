#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Revise Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Revise Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReviseReportTool : ReportingWorkflowItemTool
	{

		public ReviseReportTool()
			: base("ReviseReport")
		{
		}

		public override bool Enabled
		{
			get
			{
				return this.Context.GetOperationEnablement("ReviseResidentReport");
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			return this.Context.GetOperationEnablement("ReviseResidentReport");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			ReportingWorklistItemSummary replacementItem = ReviseResidentReport(item);

			OpenReportEditor(replacementItem);

			return true;
		}

		private ReportingWorklistItemSummary ReviseResidentReport(ReportingWorklistItemSummary item)
		{
			ReportingWorklistItemSummary result = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					ReviseResidentReportResponse response = service.ReviseResidentReport(new ReviseResidentReportRequest(item.ProcedureStepRef));
					result = response.ReplacementInterpretationStep;
				});

			return result;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Send to Resident", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Send to Resident", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.AssignSmall.png", "Icons.AssignMedium.png", "Icons.AssignLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class SendbackResidentReportTool : ReportingWorkflowItemTool
	{

		public SendbackResidentReportTool()
			: base("SendbackResidentReport")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService((IReportingWorkflowService service) =>
				service.SendbackResidentReport(new SendbackResidentReportRequest(item.ProcedureStepRef)));

			return true;
		}
	}
}
