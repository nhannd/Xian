using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyOrdersWorklist)]
	[FolderPath("Emergency Orders", true)]
	public class EROrdersFolder : EmergencyWorkflowFolder
	{
	}
}