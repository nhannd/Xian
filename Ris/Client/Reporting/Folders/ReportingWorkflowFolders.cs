#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    [ExtensionOf(typeof(ReportingMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistClass(WorklistClassNames.ReportingToBeReportedWorklist)]
    [FolderPath("To be Reported", true)]
    public class ToBeReportedFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingAssignedWorklist)]
    [FolderPath("Assigned")]
    public class AssignedFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingDraftWorklist)]
    [FolderPath("Draft")]
    public class DraftFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingInTranscriptionWorklist)]
    [FolderPath("In Transcription")]
    public class InTranscriptionFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRadiologistToBeVerifiedWorklist)]
    [FolderPath("To be Verified")]
    public class ToBeVerifiedFolder : ReportingWorkflowFolder
    {
        /// <summary>
        /// Overridden to tweak behaviour based on user permissions.
        /// </summary>
        public override string WorklistClassName
        {
            get
            {
                return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify) ?
                    WorklistClassNames.ReportingRadiologistToBeVerifiedWorklist :
                    WorklistClassNames.ReportingResidentToBeVerifiedWorklist;
                }
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingReviewResidentReportWorklist)]
    [FolderPath("Review Resident Report")]
    public class ReviewResidentReportFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRadiologistVerifiedWorklist)]
    [FolderPath("Verified")]
    public class VerifiedFolder : ReportingWorkflowFolder
    {
        /// <summary>
        /// Overridden to tweak behaviour based on user permissions.
        /// </summary>
        public override string WorklistClassName
        {
            get
            {
                return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify) ?
                    WorklistClassNames.ReportingRadiologistVerifiedWorklist :
                    WorklistClassNames.ReportingResidentVerifiedWorklist;
            }
        }
    }

    [ExtensionOf(typeof(ReportingProtocolWorkflowFolderExtensionPoint))]
    [FolderForWorklistClass(WorklistClassNames.ReportingToBeProtocolledWorklist)]
    [FolderPath("To be Protocolled", true)]
    public class ToBeProtocolledFolder : ReportingWorkflowFolder
    {
    }

	[ExtensionOf(typeof(ReportingProtocolWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.ReportingToBeApprovedProtocolWorklist)]
	[FolderPath("To be Approved", true)]
	public class ToBeApprovedFolder : ReportingWorkflowFolder
	{
	}

	[FolderForWorklistClass(WorklistClassNames.ReportingDraftProtocolWorklist)]
    [FolderPath("Draft")]
    public class DraftProtocolFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingCompletedProtocolWorklist)]
    [FolderPath("Completed")]
    public class CompletedProtocolFolder : ReportingWorkflowFolder
    {
    }

	[FolderForWorklistClass(WorklistClassNames.ReportingAwaitingApprovalProtocolWorklist)]
	[FolderPath("Awaiting Approval")]
	public class AwaitingApprovalProtocolFolder : ReportingWorkflowFolder
	{
	}

	[FolderForWorklistClass(WorklistClassNames.ReportingSuspendedProtocolWorklist)]
    [FolderPath("Suspended")]
    public class SuspendedProtocolFolder : ReportingWorkflowFolder
    {
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRejectedProtocolWorklist)]
    [FolderPath("Rejected")]
    public class RejectedProtocolFolder : ReportingWorkflowFolder
    {
    }

	[FolderPath("Search Results")]
    public class ReportingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, IReportingWorkflowService>
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
    public class ProtocollingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, IReportingWorkflowService>
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
