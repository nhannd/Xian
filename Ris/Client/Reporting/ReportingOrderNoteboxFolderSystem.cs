using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client.Reporting.Folders;

namespace ClearCanvas.Ris.Client.Reporting
{
	[ExtensionPoint]
	public class ReportingOrderNoteboxFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class ReportingOrderNoteboxItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ReportingOrderNoteboxFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public class ReportingOrderNoteboxFolderSystem : OrderNoteboxFolderSystemBase
	{
		public ReportingOrderNoteboxFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitleOrderNoteboxFolderSystem, folderExplorer,
				new ReportingOrderNoteboxFolderExtensionPoint(),
				new ReportingOrderNoteboxItemToolExtensionPoint(),
				new ReportingOrderNoteboxFolderToolExtensionPoint())
		{
			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			InboxFolder inboxFolder = new InboxFolder(this);
			inboxFolder.TotalItemCountChanged += OnPrimaryFolderCountChanged;

			this.AddFolder(inboxFolder);
			this.AddFolder(new SentItemsFolder(this));
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.ReportingOrderNoteboxFolderSystemUrl; }
		}
	}
}
