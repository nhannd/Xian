using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionPoint]
    public class TechnologistContainerFolderExtensionPoint : ExtensionPoint<IContainerFolder>
    {
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistScheduledContainerFolder : ContainerFolder
    {
        public TechnologistScheduledContainerFolder()
            : base("Scheduled", typeof(ScheduledTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCheckedInContainerFolder : ContainerFolder
    {
        public TechnologistCheckedInContainerFolder() 
            : base("Checked In", typeof(CheckedInTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistInProgressContainerFolder : ContainerFolder
    {
        public TechnologistInProgressContainerFolder()
            : base("In Progress", typeof(InProgressTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistSuspendedContainerFolder : ContainerFolder
    {
        public TechnologistSuspendedContainerFolder()
            : base("Suspended", typeof(SuspendedTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCompletedContainerFolder : ContainerFolder
    {
        public TechnologistCompletedContainerFolder() 
            : base("Completed", typeof(CompletedTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCancelledContainerFolder : ContainerFolder
    {
        public TechnologistCancelledContainerFolder() 
            : base("Cancelled", typeof(CancelledTechnologistWorkflowFolder)) { }
    }
}
