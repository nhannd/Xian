using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionPoint]
    public class RegistrationContainerFolderExtensionPoint : ExtensionPoint<IContainerFolder>
    {
    }

    [ExtensionOf(typeof(RegistrationContainerFolderExtensionPoint))]
    public class RegistrationScheduledContainerFolder : ContainerFolder
    {
        public RegistrationScheduledContainerFolder() 
            : base("Scheduled", typeof(ScheduledFolder)) { }
    }

    [ExtensionOf(typeof(RegistrationContainerFolderExtensionPoint))]
    public class RegistrationCheckedInContainerFolder : ContainerFolder
    {
        public RegistrationCheckedInContainerFolder() 
            : base("Checked In", typeof(CheckedInFolder)) { }
    }

    [ExtensionOf(typeof(RegistrationContainerFolderExtensionPoint))]
    public class RegistrationInProgressContainerFolder : ContainerFolder
    {
        public RegistrationInProgressContainerFolder() 
            : base("In Progress", typeof(InProgressFolder)) { }
    }

    [ExtensionOf(typeof(RegistrationContainerFolderExtensionPoint))]
    public class RegistrationCompletedContainerFolder 
        : ContainerFolder
    {
        public RegistrationCompletedContainerFolder() 
            : base("Completed", typeof(CompletedFolder)) { }
    }

    [ExtensionOf(typeof(RegistrationContainerFolderExtensionPoint))]
    public class RegistrationCancelledContainerFolder : ContainerFolder
    {
        public RegistrationCancelledContainerFolder() 
            : base("Cancelled", typeof(CancelledFolder)) { }
    }
}
