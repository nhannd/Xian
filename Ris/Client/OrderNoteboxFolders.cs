using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to me")]
	internal class InboxFolder : OrderNoteboxFolder
	{
		private InboxFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription)
			: base(folderSystem, folderDisplayName, folderDescription, "OrderNoteInbox")
		{
		}

		public InboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem)
			: this(orderNoteboxFolderSystem, null, null)
		{
		}

		public InboxFolder()
			: this(null)
		{
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted by me")]
	internal class SentItemsFolder : OrderNoteboxFolder
	{
		private SentItemsFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription)
			: base(folderSystem, folderDisplayName, folderDescription, "OrderNoteSentItems")
		{
		}

		public SentItemsFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem)
			: this(orderNoteboxFolderSystem, null, null)
		{
		}

		public SentItemsFolder()
			: this(null)
		{
		}
	}
}