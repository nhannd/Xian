using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to me")]
	internal class PersonalInboxFolder : OrderNoteboxFolder
	{
		public PersonalInboxFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNotePersonalInbox")
		{
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to my groups")]
	internal class GroupInboxFolder : OrderNoteboxFolder
	{
		private readonly EntityRef _groupRef;

		public GroupInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem, StaffGroupSummary staffGroup)
			: base(orderNoteboxFolderSystem, "OrderNoteGroupInbox")
		{
			_groupRef = staffGroup.StaffGroupRef;
			this.FolderPath = new Path(string.Concat(this.FolderPath.ToString(), "/", staffGroup.Name), this.ResourceResolver);
			this.Tooltip = staffGroup.Name;
            this.IsStatic = false;
		}

		public override string Id
		{
			get { return _groupRef.ToString(false); }
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
		public SentItemsFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNoteSentItems")
		{
		}
	}
}