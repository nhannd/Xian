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
			EmergencyPhysicianMainWorkflowItemToolExtensionPoint>, ISearchDataHandler
	{
		private readonly RegistrationSearchFolder _searchFolder;

		public EmergencyPhysicianMainWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitleEmergencyFolderSystem, folderExplorer)
		{
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
			{
				this.AddFolder(new EROrdersFolder(this));
			}

			this.AddFolder(_searchFolder = new RegistrationSearchFolder(this));
		}

		protected override string GetPreviewUrl()
		{
			return WebResourcesSettings.Default.EmergencyPhysicianFolderSystemUrl;
		}

		public override void OnSelectedItemDoubleClicked()
		{
			base.OnSelectedItemDoubleClicked();

			EmergencyPhysicianEmergencyOrdersConversationTool notesTool = 
				(EmergencyPhysicianEmergencyOrdersConversationTool)CollectionUtils.SelectFirst(
					this.ItemTools.Tools,
					delegate(ITool tool) { return tool is EmergencyPhysicianEmergencyOrdersConversationTool; });

			if (notesTool != null && notesTool.Enabled)
				notesTool.Open();
		}

		#region ISearchDataHandler Members

		public SearchData SearchData
		{
			set
			{
				_searchFolder.SearchData = value;
				SelectedFolder = _searchFolder;
			}
		}

		#endregion
	}
}