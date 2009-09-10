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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	public partial class OrderNoteConversationComponent
	{
		#region RecipientTableItem class

		private class RecipientTableItem
		{
			public RecipientTableItem(object staffOrGroupSummary)
			{
				this.Recipient = staffOrGroupSummary;
			}

			/// <summary>
			/// Gets or sets the recipient, which must be an instance of a <see cref="StaffGroupSummary"/> or <see cref="StaffSummary"/>.
			/// </summary>
			public object Recipient
			{
				get { return IsStaffRecipient ? (object) this.StaffSummary : this.StaffGroupSummary; }
				set
				{
					// clear both, in case value is changing from one type to the other
					this.StaffSummary = null;
					this.StaffGroupSummary = null;

					if (value is StaffGroupSummary)
						this.StaffGroupSummary = (StaffGroupSummary) value;
					else if (value is StaffSummary)
						this.StaffSummary = (StaffSummary) value;
				}
			}

			public bool IsStaffRecipient
			{
				get { return StaffSummary != null; }
			}

			public StaffSummary StaffSummary { get; private set; }

			public bool IsGroupRecipient
			{
				get { return StaffGroupSummary != null; }
			}

			public StaffGroupSummary StaffGroupSummary { get; private set; }

			public static string Format(object staffOrGroup)
			{
				return (staffOrGroup is StaffSummary)
					? StaffNameAndRoleFormat.Format(((StaffSummary)staffOrGroup))
					: (staffOrGroup is StaffGroupSummary) ? ((StaffGroupSummary)staffOrGroup).Name : string.Empty;
			}

		}

		#endregion

		private class RecipientTable : Table<Checkable<RecipientTableItem>>
		{
			private readonly OrderNoteConversationComponent _owner;

			public RecipientTable(OrderNoteConversationComponent owner)
			{
				_owner = owner;
				this.Columns.Add(new TableColumn<Checkable<RecipientTableItem>, bool>(
					"Select",
					item => item.IsChecked,
					delegate(Checkable<RecipientTableItem> item, bool value)
					{
						item.IsChecked = value;
						// bug: #2594: forces validation to refresh otherwise the screen doesn't update 
						// only when validation is visible
						if(_owner.ValidationVisible)
							_owner.ShowValidation(true);
					},
					0.4f));

				var nameColumn = new TableColumn<Checkable<RecipientTableItem>, object>(
					"Name",
					item => item.Item.Recipient,
					(x, value) => x.Item.Recipient = value,
					2.0f);
				nameColumn.ValueFormatter = RecipientTableItem.Format;
				nameColumn.CellEditor = new LookupHandlerCellEditor(new StaffAndGroupLookupHandler(owner.Host.DesktopWindow));
				this.Columns.Add(nameColumn);
			}

			public List<StaffSummary> SelectedStaff
			{
				get
				{
					return CollectionUtils.Map(
						CollectionUtils.Select(
							this.Items,
							item => item.IsChecked && item.Item.IsStaffRecipient),
						(Checkable<RecipientTableItem> item) => item.Item.StaffSummary);
				}
			}

			public List<StaffGroupSummary> SelectedStaffGroups
			{
				get
				{
					return CollectionUtils.Map(
						CollectionUtils.Select(
							this.Items,
							item => item.IsChecked && item.Item.IsGroupRecipient),
						(Checkable<RecipientTableItem> item) => item.Item.StaffGroupSummary);
				}
			}

			public void Add(object staffOrGroup, bool selected)
			{
				this.Items.Add(new Checkable<RecipientTableItem>(new RecipientTableItem(staffOrGroup), selected));
			}
		}
	}
}
