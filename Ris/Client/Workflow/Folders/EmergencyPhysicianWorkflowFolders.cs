using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Emergency
	{
		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyScheduledWorklist)]
		[FolderPath("Scheduled", true)]
		public class ScheduledFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyInProgressWorklist)]
		[FolderPath("In Progress", true)]
		public class InProgressFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyCompletedWorklist)]
		[FolderPath("Performed", true)]
		public class PerformedFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyCancelledWorklist)]
		[FolderPath("Cancelled", true)]
		public class CancelledFolder : EmergencyWorkflowFolder
		{
		}
	}
}