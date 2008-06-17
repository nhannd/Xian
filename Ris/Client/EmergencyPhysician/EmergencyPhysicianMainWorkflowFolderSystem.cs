using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Adt;
using ClearCanvas.Ris.Client.Adt.Folders;
using ClearCanvas.Ris.Client.EmergencyPhysician.Folders;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionPoint]
	public class EmergencyPhysicianMainWorkflowFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class EmergencyPhysicianMainWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class EmergencyPhysicianMainWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public class EmergencyPhysicianMainWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<EmergencyPhysicianMainWorkflowFolderExtensionPoint, EmergencyPhysicianMainWorkflowFolderToolExtensionPoint,
			EmergencyPhysicianMainWorkflowItemToolExtensionPoint>
	{
		public EmergencyPhysicianMainWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitleEmergencyFolderSystem, folderExplorer)
		{
		}

		protected override string GetPreviewUrl()
		{
			return WebResourcesSettings.Default.EmergencyPhysicianFolderSystemUrl;
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new RegistrationSearchFolder();
        }
    }
}