using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    public class ScheduledTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            :base(folderSystem, "Scheduled")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled";
        }
    }

    public class CheckedInTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Checked In", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn";
        }
    }

    public class InProgressTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress";
        }
    }

    public class CompletedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed";
        }
    }

    public class SuspendedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Suspended", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended";
        }
    }

    public class CancelledTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled", new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled";
        }
    }
}