using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Adt;
using ClearCanvas.Ris.Client.Adt.Folders;
using ClearCanvas.Ris.Client.EmergencyPhysician.Folders;

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

	public class EmergencyPhysicianMainWorkflowFolderSystem : RegistrationWorkflowFolderSystemBase, ISearchDataHandler
	{
		private readonly RegistrationSearchFolder _searchFolder;

		public EmergencyPhysicianMainWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer) 
			: base(folderExplorer,
			       new EmergencyPhysicianMainWorkflowFolderExtensionPoint(),
			       new EmergencyPhysicianMainWorkflowItemToolExtensionPoint(),
			       new EmergencyPhysicianMainWorkflowFolderToolExtensionPoint())
		{
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.ViewUnfilteredWorkflowFolders))
			{
				this.AddFolder(new EROrdersFolder(this));
			}

			this.AddFolder(_searchFolder = new RegistrationSearchFolder(this));
			folderExplorer.RegisterSearchDataHandler(this);
		}

		public override string DisplayName
		{
			get { return SR.TitleEmergencyPhysicianHome; }
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.EmergencyPhysicianFolderSystemUrl; }
		}

		public SearchData SearchData
		{
			set
			{
				_searchFolder.SearchData = value;
				SelectedFolder = _searchFolder;
			}
		}
	}
}