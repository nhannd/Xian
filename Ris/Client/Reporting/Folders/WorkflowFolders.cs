using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType("Reporting - To Be Reported")]
    public class ToBeReportedFolder : ReportingWorkflowFolder
    {
        public ToBeReportedFolder(ReportingWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ToBeReported";
        }

        public ToBeReportedFolder(ReportingWorkflowFolderSystem folderSystem)
            : this(folderSystem, "To be Reported", null)
        {
        }

        public ToBeReportedFolder()
            : this(null)
        {
        }
    }

    public class InProgressFolder : ReportingWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ReportingWorklistItem>>
        {
        }

        public InProgressFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+InProgress";
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
}
