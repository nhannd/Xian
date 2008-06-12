using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Adt;
using ClearCanvas.Ris.Client.EmergencyPhysician;

namespace ClearCanvas.Ris.Client.EmergencyPhysician.Folders
{
	[ExtensionOf(typeof(EmergencyPhysicianMainWorkflowFolderExtensionPoint))]
	[FolderForWorklistClass(WorklistClassNames.EmergencyOrdersWorklist)]
	[FolderPath("Emergency Orders", true)]
	public class EROrdersFolder : EmergencyPhysicianWorkflowFolder
	{
		public EROrdersFolder(WorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
			: base(folderSystem, folderDisplayName, folderDescription, worklistRef)
		{
		}

		public EROrdersFolder(WorkflowFolderSystem folderSystem)
			: this(folderSystem, null, null, null)
		{
		}

		public EROrdersFolder()
			: this(null)
		{
		}
	}
}