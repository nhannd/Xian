#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended.Folders
{
	public class Reporting
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ProtocollingAdminAssignedWorklist)]
		[FolderPath("Active Protocolling Items", true)]
		[FolderDescription("ProtocollingAdminAssignedFolderDescription")]
		public class ProtocollingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingToBeProtocolledWorklist)]
		[FolderPath("To be Protocolled", true)]
		[FolderDescription("ReportingToBeProtocolledFolderDescription")]
		public class ToBeProtocolledFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAssignedProtocolWorklist)]
		[FolderPath("My Items/To be Protocolled")]
		[FolderDescription("ReportingAssignedToBeProtocolFolderDescription")]
		public class AssignedToBeProtocolFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingToBeReviewedProtocolWorklist)]
		[FolderPath("To be Reviewed", true)]
		[FolderDescription("ReportingToBeReviewedProtocolFolderDescription")]
		public class ToBeReviewedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAssignedReviewProtocolWorklist)]
		[FolderPath("My Items/To be Reviewed")]
		[FolderDescription("ReportingAssignedForReviewProtocolFolderDescription")]
		public class AssignedForReviewProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingDraftProtocolWorklist)]
		[FolderPath("My Items/Draft")]
		[FolderDescription("ReportingDraftProtocolFolderDescription")]
		public class DraftProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingCompletedProtocolWorklist)]
		[FolderPath("My Items/Completed")]
		[FolderDescription("ReportingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAwaitingApprovalProtocolWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		[FolderDescription("ReportingAwaitingApprovalProtocolFolderDescription")]
		public class AwaitingApprovalProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingRejectedProtocolWorklist)]
		[FolderPath("My Items/Rejected")]
		[FolderDescription("ReportingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class ProtocollingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public ProtocollingSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ProtocolAssignmentStep"; }
			}
		}
	}
}
