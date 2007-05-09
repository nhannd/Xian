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
        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Checked In")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn";
        }
    }

    public class InProgressTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress";
        }
    }

    public class CompletedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed";
        }
    }

    public class SuspendedTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Suspended")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended";
        }
    }

    public class CancelledTechnologistWorkflowFolder : TechnologistWorkflowFolderBase
    {
        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled";
        }
    }
}