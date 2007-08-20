using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistScheduledWorklist)]
    public class ScheduledTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, null)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled";
        }

        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Scheduled", null, null)
        {
        }

        public ScheduledTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCheckedInWorklist)]
    public class CheckedInTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn";
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Checked In", null, null)
        {
        }

        public CheckedInTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistInProgressWorklist)]
    public class InProgressTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress";
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "In Progress", null, null)
        {
        }

        public InProgressTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCompletedWorklist)]
    public class CompletedTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed";
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Completed", null, null)
        {
        }

        public CompletedTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistSuspendedWorklist)]
    public class SuspendedTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended";
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Suspended", null, null)
        {
        }

        public SuspendedTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCancelledWorklist)]
    public class CancelledTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled";
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Cancelled", null, null)
        {
        }

        public CancelledTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }
}