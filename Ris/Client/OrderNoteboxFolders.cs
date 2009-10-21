#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
	internal class SentItemsFolder : OrderNoteboxFolder
	{
		public SentItemsFolder(OrderNoteboxFolderSystem folderSystem)
			: base(folderSystem, "OrderNoteSentItems")
		{
		}
	}
}