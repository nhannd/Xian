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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class StaffSelectorEditorComponent : WorklistSelectorEditorComponent<StaffSummary, StaffSelectorTable>
	{
		public class DummyItem : StaffSummary
		{
			public DummyItem()
			{
				this.Name = new PersonNameDetail();
				this.Name.FamilyName = SR.DummyItemUser;
				this.StaffId = "";
				this.StaffType = new EnumValueInfo("", "");
				this.StaffRef = new EntityRef(typeof(DummyItem), new object(), 0);
			}
		}

		private static readonly StaffSummary _currentUserItem = new DummyItem();

		private static IEnumerable<StaffSummary> CollectionAndCurrentUser(IEnumerable<StaffSummary> items)
		{
			List<StaffSummary> a = new List<StaffSummary>();
			a.Add(_currentUserItem);
			a.AddRange(items);
			return a;
		}

		public StaffSelectorEditorComponent(IEnumerable<StaffSummary> allItems, IEnumerable<StaffSummary> selectedItems, bool includeCurrentUser)
			: base(
				CollectionAndCurrentUser(allItems), 
				includeCurrentUser ? CollectionAndCurrentUser(selectedItems) : selectedItems, 
				delegate(StaffSummary s) { return s.StaffRef; })
		{
		}

		public bool IncludeCurrentUser
		{
			get { return base.SelectedItems.Contains(_currentUserItem); }
		}

		public override IList<StaffSummary> SelectedItems
		{
			get
			{
				return CollectionUtils.Select(base.SelectedItems, delegate(StaffSummary staff) { return staff != _currentUserItem; });
			}
		}
	}
}