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
    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.ReportingToBeReportedWorklist)]
    public class ToBeReportedFolder : ReportingWorkflowFolder
    {
        public ToBeReportedFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ToBeReported";
        }

        public ToBeReportedFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "To be Reported", null, null)
        {
        }

        public ToBeReportedFolder()
            : this(null)
        {
        }
    }

    public class DraftFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public DraftFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Draft", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+Draft";
        }
    }

    public class InTranscriptionFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public InTranscriptionFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Transcription", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+InTranscription";
        }
    }

    public class ToBeVerifiedFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public ToBeVerifiedFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "To be Verified", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ToBeVerified";
        }
    }

    public class MyResidentToBeVerifyFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public MyResidentToBeVerifyFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Review Resident Report", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+MyResidentToBeVerified";
        }
    }

    public class VerifiedFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public VerifiedFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Verified", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+Verified";
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.ReportingToBeProtocolledWorklist)]
    public class ToBeProtocolledFolder : ReportingWorkflowFolder
    {
        public ToBeProtocolledFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ToBeProtocolled";
        }

        public ToBeProtocolledFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "To be Protocolled", null, null)
        {
        }

        public ToBeProtocolledFolder()
            : this(null)
        {
        }
    }

    public class ToBeApprovedFolder : ReportingWorkflowFolder
    {
        public ToBeApprovedFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ToBeApproved";
        }

        public ToBeApprovedFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "To be Approved", null, null)
        {
        }

        public ToBeApprovedFolder()
            : this(null)
        {
        }
    }

    public class CompletedProtocolFolder : ReportingWorkflowFolder
    {
        public CompletedProtocolFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+CompletedProtocol";
        }

        public CompletedProtocolFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Completed Protocols", null, null)
        {
        }

        public CompletedProtocolFolder()
            : this(null)
        {
        }
    }

    public class SuspendedProtocolFolder : ReportingWorkflowFolder
    {
        public SuspendedProtocolFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+SuspendedProtocol";
        }

        public SuspendedProtocolFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Suspended Protocols", null, null)
        {
        }

        public SuspendedProtocolFolder()
            : this(null)
        {
        }
    }

    public class SearchFolder : ReportingWorkflowFolder
    {
        private SearchData _searchData;

        public SearchFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
            this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
            this.ClosedIconSet = new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
            if (this.IsOpen)
                this.IconSet = this.OpenIconSet;
            else
                this.IconSet = this.ClosedIconSet;

            this.RefreshTime = 0;
            //this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+Search";
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
                    SearchResponse response = service.Search(new SearchRequest(this.SearchData));
                    worklistItems = response.WorklistItems;
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
