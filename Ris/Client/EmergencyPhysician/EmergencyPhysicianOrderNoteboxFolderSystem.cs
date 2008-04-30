using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.EmergencyPhysician.Folders;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	[ExtensionPoint]
	public class EmergencyPhysicianOrderNoteboxFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class EmergencyPhysicianOrderNoteboxItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class EmergencyPhysicianOrderNoteboxFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public class EmergencyPhysicianOrderNoteboxFolderSystem : OrderNoteboxFolderSystemBase
	{
		public EmergencyPhysicianOrderNoteboxFolderSystem(IFolderExplorerToolContext folderExplorer) 
			: base(folderExplorer, 
			       new EmergencyPhysicianOrderNoteboxFolderExtensionPoint(), 
			       new EmergencyPhysicianOrderNoteboxItemToolExtensionPoint(), 
			       new EmergencyPhysicianOrderNoteboxFolderToolExtensionPoint())
		{
			this.AddFolder(new InboxFolder(this));
			this.AddFolder(new SentItemsFolder(this));
		}

		public override string DisplayName
		{
			get { return SR.TitleOrderNoteboxFolderSystem; }
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.EmergencyPhysicianOrderNoteboxFolderSystemUrl; }
		}
	}
}