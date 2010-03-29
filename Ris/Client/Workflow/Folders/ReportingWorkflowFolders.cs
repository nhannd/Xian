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

		#region Protocol Worklists

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeProtocolledWorklist)]
		[FolderPath("To be Protocolled", true)]
		[FolderDescription("ReportingToBeProtocolledFolderDescription")]
		public class ToBeProtocolledFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedProtocolWorklist)]
		[FolderPath("My Items/To be Protocolled")]
		[FolderDescription("ReportingAssignedToBeProtocolFolderDescription")]
		public class AssignedToBeProtocolFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeReviewedProtocolWorklist)]
		[FolderPath("To be Reviewed", true)]
		[FolderDescription("ReportingToBeReviewedProtocolFolderDescription")]
		public class ToBeReviewedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedReviewProtocolWorklist)]
		[FolderPath("My Items/To be Reviewed")]
		[FolderDescription("ReportingAssignedForReviewProtocolFolderDescription")]
		public class AssignedForReviewProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingDraftProtocolWorklist)]
		[FolderPath("My Items/Draft")]
		[FolderDescription("ReportingDraftProtocolFolderDescription")]
		public class DraftProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingCompletedProtocolWorklist)]
		[FolderPath("My Items/Completed")]
		[FolderDescription("ReportingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAwaitingApprovalProtocolWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		[FolderDescription("ReportingAwaitingApprovalProtocolFolderDescription")]
		public class AwaitingApprovalProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingRejectedProtocolWorklist)]
		[FolderPath("My Items/Rejected")]
		[FolderDescription("ReportingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : ReportingWorkflowFolder
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
