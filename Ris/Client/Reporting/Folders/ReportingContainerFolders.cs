using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    [ExtensionPoint]
    public class ReportingContainerFolderExtensionPoint : ExtensionPoint<IContainerFolder>
    {
    }

    [ExtensionOf(typeof(ReportingContainerFolderExtensionPoint))]
    public class ReportingToBeReportedContainerFolder : ContainerFolder
    {
        public ReportingToBeReportedContainerFolder() 
            : base("To Be Reported", typeof(ToBeReportedFolder)) { }
    }
}
