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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
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
				return this.Context.GetOperationEnablement("ReviseResidentReport")
					|| this.Context.GetOperationEnablement("ReviseUnpublishedReport");
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			return this.Context.GetOperationEnablement("ReviseResidentReport")
				|| this.Context.GetOperationEnablement("ReviseUnpublishedReport");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			ReportingWorklistItemSummary replacementItem = null;

			if (this.Context.GetOperationEnablement("ReviseResidentReport"))
			{
				Platform.GetService(
					delegate(IReportingWorkflowService service)
					{
						var response = service.ReviseResidentReport(new ReviseResidentReportRequest(item.ProcedureStepRef));
						replacementItem = response.ReplacementInterpretationStep;
					});
			}
			else if (this.Context.GetOperationEnablement("ReviseUnpublishedReport"))
			{
				Platform.GetService(
					delegate(IReportingWorkflowService service)
					{
						var response = service.ReviseUnpublishedReport(new ReviseUnpublishedReportRequest(item.ProcedureStepRef));
						replacementItem = response.ReplacementVerificationStep;
					});
			}

			OpenReportEditor(replacementItem);

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Return to Interpreter", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Return to Interpreter", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.AssignSmall.png", "Icons.AssignMedium.png", "Icons.AssignLarge.png")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReturnToInterpreterTool : ReportingWorkflowItemTool
	{

		public ReturnToInterpreterTool()
			: base("ReturnToInterpreter")
		{
		}

		public event EventHandler VisibleChanged;
		public bool Visible
		{
			get { return new WorkflowConfigurationReader().EnableInterpretationReviewWorkflow; }
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService((IReportingWorkflowService service) =>
				service.ReturnToInterpreter(new ReturnToInterpreterRequest(item.ProcedureStepRef)));

			return true;
		}
	}
}
