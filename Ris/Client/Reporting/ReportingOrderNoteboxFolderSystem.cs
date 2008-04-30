using ClearCanvas.Common;
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
				: base(folderExplorer,
				new ReportingOrderNoteboxFolderExtensionPoint(),
				new ReportingOrderNoteboxItemToolExtensionPoint(),
				new ReportingOrderNoteboxFolderToolExtensionPoint())
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
				get { return WebResourcesSettings.Default.ReportingOrderNoteboxFolderSystemUrl; }
			}
		}
}
