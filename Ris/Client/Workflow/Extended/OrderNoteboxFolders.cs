#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to me")]
	[FolderDescription("OrderNotesPersonalInboxFolderDescription")]
	internal class PersonalInboxFolder : OrderNoteboxFolder
	{
		public PersonalInboxFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNotePersonalInbox")
		{
		}
	}

	[ExtensionOf(typeof(OrderNoteboxFolderExtensionPoint))]
	[FolderPath("Posted to my groups")]
	[FolderDescription("OrderNotesGroupInboxFolderDescription")]
	internal class GroupInboxFolder : OrderNoteboxFolder
	{
		private readonly EntityRef _groupRef;

		public GroupInboxFolder(OrderNoteboxFolderSystem orderNoteboxFolderSystem, StaffGroupSummary staffGroup)
			: base(orderNoteboxFolderSystem, "OrderNoteGroupInbox")
		{
			_groupRef = staffGroup.StaffGroupRef;
			this.FolderPath = this.FolderPath.Append(new PathSegment(staffGroup.Name, this.ResourceResolver));
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
	[FolderDescription("OrderNotesSentItemsFolderDescription")]
	internal class SentItemsFolder : OrderNoteboxFolder
	{
		public SentItemsFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNoteSentItems")
		{
		}
	}
}