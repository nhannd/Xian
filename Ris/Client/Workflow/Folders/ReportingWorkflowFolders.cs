#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Reporting
	{
		#region Reporting Worklists

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeReportedWorklist)]
		[FolderPath("To be Reported", true)]
		[FolderDescription("ReportingToBeReportedFolderDescription")]
		public class ToBeReportedFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedWorklist)]
		[FolderPath("My Items/To be Reported")]
		[FolderDescription("ReportingAssignedFolderDescription")]
		public class AssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeReviewedReportWorklist)]
		[FolderPath("To be Reviewed", true)]
		[FolderDescription("ReportingToBeReviewedFolderDescription")]
		public class ToBeReviewedFolder : ReportingWorkflowFolder
		{
			public ToBeReviewedFolder()
			{
				if(!new WorkflowConfigurationReader().EnableInterpretationReviewWorkflow)
					throw new NotSupportedException();
			}
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedReviewWorklist)]
		[FolderPath("My Items/To be Reviewed")]
		[FolderDescription("ReportingAssignedForReviewFolderDescription")]
		public class AssignedForReviewFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingDraftWorklist)]
		[FolderPath("My Items/Draft")]
		[FolderDescription("ReportingDraftFolderDescription")]
		public class DraftFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingInTranscriptionWorklist)]
		[FolderPath("My Items/In Transcription")]
		[FolderDescription("ReportingInTranscriptionFolderDescription")]
		public class InTranscriptionFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingReviewTranscriptionWorklist)]
		[FolderPath("My Items/Review Transcription")]
		[FolderDescription("ReportingReviewTranscriptionFolderDescription")]
		public class ReviewTranscriptionFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAwaitingReviewWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		[FolderDescription("ReportingAwaitingReviewFolderDescription")]
		public class AwaitingReviewFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingVerifiedWorklist)]
		[FolderPath("My Items/Verified")]
		[FolderDescription("ReportingVerifiedFolderDescription")]
		public class VerifiedFolder : ReportingWorkflowFolder
		{
		}

		#endregion

		#region Reporting Tracking Worklists

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingActiveWorklist)]
		[FolderPath("Tracking/Active", true)]
		[FolderDescription("ReportingTrackingActiveFolderDescription")]
		public class ReportingTrackingActiveFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingDraftWorklist)]
		[FolderPath("Tracking/Draft", true)]
		[FolderDescription("ReportingTrackingDraftFolderDescription")]
		public class ReportingTrackingDraftFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingPreliminaryWorklist)]
		[FolderPath("Tracking/Preliminary", true)]
		[FolderDescription("ReportingTrackingPreliminaryFolderDescription")]
		public class ReportingTrackingPreliminaryFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingFinalWorklist)]
		[FolderPath("Tracking/Finalized", true)]
		[FolderDescription("ReportingTrackingFinalFolderDescription")]
		public class ReportingTrackingFinalFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingCorrectedWorklist)]
		[FolderPath("Tracking/Corrected", true)]
		[FolderDescription("ReportingTrackingCorrectedFolderDescription")]
		public class ReportingTrackingCorrectedFolder : ReportingWorkflowFolder
		{
		}

		#endregion


		[FolderPath("Search Results")]
		public class ReportingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public ReportingSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ReportingProcedureStep"; }
			}

		}
	}
}
