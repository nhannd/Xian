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

    public class ResidentToBeVerifyFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public ResidentToBeVerifyFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Resident", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ResidentToBeVerified";
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
