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
        public ToBeReportedFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public ToBeReportedFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public ToBeReportedFolder()
            : this(null)
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingAssignedWorklist)]
    [FolderPath("Assigned")]
    public class AssignedFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public AssignedFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingDraftWorklist)]
    [FolderPath("Draft")]
    public class DraftFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public DraftFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingInTranscriptionWorklist)]
    [FolderPath("In Transcription")]
    public class InTranscriptionFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public InTranscriptionFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRadiologistToBeVerifiedWorklist)]
    [FolderPath("To be Verified")]
    public class ToBeVerifiedFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public ToBeVerifiedFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }

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
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public ReviewResidentReportFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRadiologistVerifiedWorklist)]
    [FolderPath("Verified")]
    public class VerifiedFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public VerifiedFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null, new DropHandlerExtensionPoint())
        {
        }

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
        public ToBeProtocolledFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public ToBeProtocolledFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public ToBeProtocolledFolder()
            : this(null)
        {
        }
    }

	[ExtensionOf(typeof(ReportingProtocolWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.ReportingToBeApprovedProtocolWorklist)]
	[FolderPath("To be Approved", true)]
	public class ToBeApprovedFolder : ReportingWorkflowFolder
	{
		public ToBeApprovedFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
			: base(folderSystem, folderDisplayName, folderDescription, worklistRef)
		{
		}

		public ToBeApprovedFolder(ReportingWorkflowFolderSystemBase folderSystem)
			: this(folderSystem, null, null, null)
		{
		}

		public ToBeApprovedFolder()
			: this(null)
		{
		}
	}

	[FolderForWorklistClass(WorklistClassNames.ReportingDraftProtocolWorklist)]
    [FolderPath("Draft")]
    public class DraftProtocolFolder : ReportingWorkflowFolder
    {
        public DraftProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public DraftProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public DraftProtocolFolder()
            : this(null)
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingCompletedProtocolWorklist)]
    [FolderPath("Completed")]
    public class CompletedProtocolFolder : ReportingWorkflowFolder
    {
        public CompletedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public CompletedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public CompletedProtocolFolder()
            : this(null)
        {
        }
    }

	[FolderForWorklistClass(WorklistClassNames.ReportingAwaitingApprovalProtocolWorklist)]
	[FolderPath("Awaiting Approval")]
	public class AwaitingApprovalProtocolFolder : ReportingWorkflowFolder
	{
		public AwaitingApprovalProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
			: base(folderSystem, folderDisplayName, folderDescription, worklistRef)
		{
		}

		public AwaitingApprovalProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem)
			: this(folderSystem, null, null, null)
		{
		}

		public AwaitingApprovalProtocolFolder()
			: this(null)
		{
		}
	}

	[FolderForWorklistClass(WorklistClassNames.ReportingSuspendedProtocolWorklist)]
    [FolderPath("Suspended")]
    public class SuspendedProtocolFolder : ReportingWorkflowFolder
    {
        public SuspendedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public SuspendedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public SuspendedProtocolFolder()
            : this(null)
        {
        }
    }

    [FolderForWorklistClass(WorklistClassNames.ReportingRejectedProtocolWorklist)]
    [FolderPath("Rejected")]
    public class RejectedProtocolFolder : ReportingWorkflowFolder
    {
        public RejectedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
        }

        public RejectedProtocolFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public RejectedProtocolFolder()
            : this(null)
        {
        }
    }

	[FolderPath("Search Results")]
    public class ReportingSearchFolder : SearchResultsFolder<ReportingWorklistItem>
    {
        public ReportingSearchFolder(ReportingWorkflowFolderSystemBase folderSystem)
			: base(folderSystem, new ReportingWorklistTable())
        {
        }

		protected override TextQueryResponse<ReportingWorklistItem> DoQuery(string query, int specificityThreshold)
		{
			TextQueryResponse<ReportingWorklistItem> response = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
					//it is the only way to get things working right now
					response = service.SearchWorklists(new WorklistTextQueryRequest(query, specificityThreshold, "ReportingProcedureStep"));
				});
			return response;
		}

    }

	[FolderPath("Search Results")]
	public class ProtocollingSearchFolder : SearchResultsFolder<ReportingWorklistItem>
	{
		public ProtocollingSearchFolder(ReportingWorkflowFolderSystemBase folderSystem)
			: base(folderSystem, new ReportingWorklistTable())
		{
		}

		protected override TextQueryResponse<ReportingWorklistItem> DoQuery(string query, int specificityThreshold)
		{
			TextQueryResponse<ReportingWorklistItem> response = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
					//it is the only way to get things working right now
					response = service.SearchWorklists(new WorklistTextQueryRequest(query, specificityThreshold, "ProtocolAssignmentStep"));
				});
			return response;
		}

	}
}
