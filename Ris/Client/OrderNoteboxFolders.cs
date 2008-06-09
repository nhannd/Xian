using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

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
		private readonly EntityRef _groupRef;

		private GroupInboxFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription)
			: base(folderSystem, folderDisplayName, folderDescription, "OrderNoteGroupInbox")
		{
		}

		public GroupInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem, StaffGroupSummary staffGroup)
			: this(orderNoteboxFolderSystem, staffGroup.Name, staffGroup.Name)
		{
			_groupRef = staffGroup.StaffGroupRef;
		}

		protected override void PrepareQueryRequest(QueryNoteboxRequest request)
		{
			base.PrepareQueryRequest(request);

			request.StaffGroupRef = _groupRef;
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