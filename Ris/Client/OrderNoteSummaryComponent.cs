#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteSummaryComponent : SummaryComponentBase<OrderNoteDetail, OrderNoteTable>
	{
		private readonly OrderNoteCategory _category;
		private IList<OrderNoteDetail> _notes;
		private readonly bool _canEdit;

		/// <summary>
		/// Constructor allowing edits.
		/// </summary>
		/// <param name="category"></param>
		public OrderNoteSummaryComponent(OrderNoteCategory category)
			: this(category, true)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="category">Specifies the category of order notes to display, and the category under which new notes are placed.</param>
		/// <param name="canEdit">Specifies if the component is editable.  If not, all action buttons are disabled.</param>
		public OrderNoteSummaryComponent(OrderNoteCategory category, bool canEdit)
			: base(false)
		{
			Platform.CheckForNullReference(category, "category");

			_canEdit = canEdit;
			_category = category;
			_notes = new List<OrderNoteDetail>();

			this.Table.UpdateNoteClickLinkDelegate = UpdateOrderNoteDetail;
		}

		public IList<OrderNoteDetail> Notes
		{
			get { return _notes; }
			set
			{
				_notes = value;
				this.Table.Items.Clear();
				this.Table.Items.AddRange(CollectionUtils.Select(_notes, d => d.Category == _category.Key));
			}
		}

		#region Overrides

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override bool SupportsPaging
		{
			get { return false; }
		}

		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.Enabled = _canEdit;
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			var selectedNote = CollectionUtils.FirstElement(this.SelectedItems);
			if (selectedNote == null)
				return;

			// only un-posted notes can be edited or deleted
			this.ActionModel.Edit.Enabled &= selectedNote.PostTime == null;
			this.ActionModel.Delete.Enabled &= selectedNote.PostTime == null;
		}

		protected override IList<OrderNoteDetail> ListItems(int firstRow, int maxRows)
		{
			// Only list the notes of the specified category.
			return CollectionUtils.Select(_notes, d => d.Category == _category.Key);
		}

		protected override bool AddItems(out IList<OrderNoteDetail> addedItems)
		{
			addedItems = new List<OrderNoteDetail>();

			var newNote = new OrderNoteDetail(_category.Key, "", null, false, null, null);
			var editor = new OrderNoteEditorComponent(newNote);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				addedItems.Add(newNote);
				_notes.Add(newNote);
				return true;
			}

			return false;
		}

		protected override bool EditItems(IList<OrderNoteDetail> items, out IList<OrderNoteDetail> editedItems)
		{
			editedItems = new List<OrderNoteDetail>();

			var originalNote = CollectionUtils.FirstElement(items);
			// manually clone ordernote
			var editedNote = new OrderNoteDetail(
				originalNote.OrderNoteRef,
				originalNote.Category,
				originalNote.CreationTime,
				originalNote.PostTime,
				originalNote.Author,
				originalNote.OnBehalfOfGroup,
				originalNote.Urgent,
				originalNote.StaffRecipients,
				originalNote.GroupRecipients,
				originalNote.NoteBody,
				originalNote.CanAcknowledge
				);

			var editor = new OrderNoteEditorComponent(editedNote);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				editedItems.Add(editedNote);

				// Preserve the order of the items
				var index = _notes.IndexOf(originalNote);
				_notes.Insert(index, editedNote);
				_notes.Remove(originalNote);

				return true;
			}

			return false;
		}

		protected override bool DeleteItems(IList<OrderNoteDetail> items, out IList<OrderNoteDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<OrderNoteDetail>();

			foreach (var item in items)
			{
				deletedItems.Add(item);
				_notes.Remove(item);
			}

			return deletedItems.Count > 0;
		}

		protected override bool IsSameItem(OrderNoteDetail x, OrderNoteDetail y)
		{
			if (ReferenceEquals(x, y))
				return true;

			// if only one is null, they are not the same
			if (x.OrderNoteRef == null || y.OrderNoteRef == null)
				return false;

			return x.OrderNoteRef.Equals(y.OrderNoteRef, true);
		}

		#endregion

		private void UpdateOrderNoteDetail(OrderNoteDetail note)
		{
			this.SummarySelection = new Selection(note);
			this.EditSelectedItems();
		}
	}
}
