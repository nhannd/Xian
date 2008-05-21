using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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
			: base(SR.TitleOrderNoteboxFolderSystem, folderExplorer, 
			       new EmergencyPhysicianOrderNoteboxFolderExtensionPoint(), 
			       new EmergencyPhysicianOrderNoteboxItemToolExtensionPoint(), 
			       new EmergencyPhysicianOrderNoteboxFolderToolExtensionPoint())
		{
			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			InboxFolder inboxFolder = new InboxFolder(this);
			inboxFolder.TotalItemCountChanged += OnPrimaryFolderCountChanged;

			this.AddFolder(inboxFolder);
			this.AddFolder(new SentItemsFolder(this));
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.EmergencyPhysicianOrderNoteboxFolderSystemUrl; }
		}
	}
}