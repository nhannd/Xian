using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to me")]
	internal class PersonalInboxFolder : OrderNoteboxFolder
	{
		private PersonalInboxFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription)
			: base(folderSystem, folderDisplayName, folderDescription, "OrderNotePersonalInbox")
		{
		}

		public PersonalInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem)
			: this(orderNoteboxFolderSystem, null, null)
		{
		}

		public PersonalInboxFolder()
			: this(null)
		{
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to my groups")]
	internal class GroupInboxFolder : OrderNoteboxFolder
	{
		private GroupInboxFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription)
			: base(folderSystem, folderDisplayName, folderDescription, "OrderNoteGroupInbox")
		{
		}

		public GroupInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem)
			: this(orderNoteboxFolderSystem, null, null)
		{
		}

		public GroupInboxFolder()
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