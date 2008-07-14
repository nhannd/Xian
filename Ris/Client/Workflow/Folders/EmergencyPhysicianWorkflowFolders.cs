using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyScheduledWorklist)]
	[FolderPath("Scheduled", true)]
	public class EmergencyScheduledFolder : EmergencyWorkflowFolder
	{
	}

	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyInProgressWorklist)]
	[FolderPath("In Progress", true)]
	public class EmergencyInProgressFolder : EmergencyWorkflowFolder
	{
	}

	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyCompletedWorklist)]
	[FolderPath("Completed", true)]
	public class EmergencyCompletedFolder : EmergencyWorkflowFolder
	{
	}

	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyCancelledWorklist)]
	[FolderPath("Cancelled", true)]
	public class EmergencyCancelledFolder : EmergencyWorkflowFolder
	{
	}
}