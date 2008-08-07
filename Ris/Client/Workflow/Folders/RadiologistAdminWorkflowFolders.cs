using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public abstract class RadiologistAdmin
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminAssignedWorklist)]
		[FolderPath("Reporting Admin", true)]
		public class ReportingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ProtocollingAdminAssignedWorklist)]
		[FolderPath("Protocolling Admin", true)]
		public class ProtocollingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}
	}
}
