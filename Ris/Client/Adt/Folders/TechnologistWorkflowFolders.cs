using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    public class ScheduledTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, null)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled";
        }

        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Scheduled", null)
        {
        }
    }

    public class CheckedInTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn";
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Checked In", null)
        {
        }
    }

    public class InProgressTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress";
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "In Progress", null)
        {
        }
    }

    public class CompletedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed";
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
        : this(folderSystem, "Completed", null)
        {
        }
    }

    public class SuspendedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended";
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Suspended", null)
        {
        }
    }

    public class CancelledTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled";
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Cancelled", null)
        {
        }
    }
}