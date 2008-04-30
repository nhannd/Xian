using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
	[ExtensionOf(typeof(ReportingOrderNoteboxFolderExtensionPoint))]
	[FolderPath("Inbox")]
	internal class InboxFolder : OrderNoteboxFolder
	{
		private InboxFolder(OrderNoteboxFolderSystemBase folderSystemBase, string folderDisplayName, string folderDescription)
			: base(folderSystemBase, folderDisplayName, folderDescription, "Inbox")
		{
		}

		public InboxFolder(OrderNoteboxFolderSystemBase orderNoteboxFolderSystemBase)
			: this(orderNoteboxFolderSystemBase, null, null)
		{
		}

		public InboxFolder()
			: this(null)
		{
		}
	}

	[ExtensionOf(typeof(ReportingOrderNoteboxFolderExtensionPoint))]
	[FolderPath("Sent Items")]
	internal class SentItemsFolder : OrderNoteboxFolder
	{
		private SentItemsFolder(OrderNoteboxFolderSystemBase folderSystemBase, string folderDisplayName, string folderDescription)
			: base(folderSystemBase, folderDisplayName, folderDescription, "SentItems")
		{
		}

		public SentItemsFolder(OrderNoteboxFolderSystemBase orderNoteboxFolderSystemBase)
			: this(orderNoteboxFolderSystemBase, null, null)
		{
		}

		public SentItemsFolder()
			: this(null)
		{
		}
	}
}
