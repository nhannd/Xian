using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Workflow;
using ClearCanvas.Ris.Client.Emergency;

namespace ClearCanvas.Ris.Client.Emergency.Folders
{
	[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyOrdersWorklist)]
	[FolderPath("Emergency Orders", true)]
	public class EROrdersFolder : EmergencyWorkflowFolder
	{
	}
}