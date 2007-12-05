#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    [ExtensionOf(typeof(ReportingMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.ReportingToBeReportedWorklist)]
    [FolderPath("Reporting/To be Reported", true)]
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

    [FolderForWorklistType(WorklistTokens.ReportingDraftWorklist)]
    [FolderPath("Reporting/Draft")]
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

    [FolderForWorklistType(WorklistTokens.ReportingInTranscriptionWorklist)]
    [FolderPath("Reporting/In Transcriptioin")]
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

    [FolderForWorklistType(WorklistTokens.ReportingToBeVerifiedWorklist)]
    [FolderPath("Reporting/To be Verified")]
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
    }

    [FolderForWorklistType(WorklistTokens.ReportingReviewResidentReport)]
    [FolderPath("Reporting/Review Resident Report")]
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

    [FolderForWorklistType(WorklistTokens.ReportingVerifiedWorklist)]
    [FolderPath("Reporting/Verified")]
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
    }

    [ExtensionOf(typeof(ReportingProtocolWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.ReportingToBeProtocolledWorklist)]
    [FolderPath("Protocolling/To be Protocolled", true)]
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

    [FolderForWorklistType(WorklistTokens.ReportingToBeApprovedWorklist)]
    [FolderPath("Protocolling/To be Approved")]
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

    [FolderForWorklistType(WorklistTokens.ReportingDraftProtocolWorklist)]
    [FolderPath("Protocolling/Draft")]
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

    [FolderForWorklistType(WorklistTokens.ReportingCompletedProtocolWorklist)]
    [FolderPath("Protocolling/Completed")]
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

    [FolderForWorklistType(WorklistTokens.ReportingSuspendedProtocolWorklist)]
    [FolderPath("Protocolling/Suspended")]
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

    [FolderForWorklistType(WorklistTokens.ReportingRejectedProtocolWorklist)]
    [FolderPath("Protocolling/Rejected")]
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

    [FolderPath("Reporting/Search")]
    public class SearchFolder : ReportingWorkflowFolder
    {
        private SearchData _searchData;

        public SearchFolder(ReportingWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null)
        {
            this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
            this.ClosedIconSet = new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
            this.IconSet = this.ClosedIconSet;
            this.RefreshTime = 0;
        }

        public SearchData SearchData
        {
            get { return _searchData; }
            set
            {
                _searchData = value;
                this.Refresh();
            }
        }

        protected override bool CanQuery()
        {
            if (this.SearchData != null)
                return true;

            return false;
        }

        protected override IList<ReportingWorklistItem> QueryItems()
        {
            List<ReportingWorklistItem> worklistItems = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    SearchRequest request = new SearchRequest();
                    request.TextQuery = this.SearchData.TextSearch;
                    request.ShowActivOnly = this.SearchData.ShowActiveOnly;
                    TextQueryResponse<ReportingWorklistItem> response = service.Search(request);
                    worklistItems = response.Matches;
                });

            if (worklistItems == null)
                worklistItems = new List<ReportingWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            return this.ItemCount;
        }
    }
}
