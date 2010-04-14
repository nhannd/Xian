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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PatientNoteSummaryComponent : SummaryComponentBase<PatientNoteDetail, PatientNoteTable>
	{
		private readonly List<PatientNoteCategorySummary> _noteCategoryChoices;
		private IList<PatientNoteDetail> _notes;

		public PatientNoteSummaryComponent(List<PatientNoteCategorySummary> categoryChoices)
			: base(false)
		{
			_noteCategoryChoices = categoryChoices;
		}

		public IList<PatientNoteDetail> Subject
		{
			get { return _notes; }
			set { _notes = value; }
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

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			var selectedNote = CollectionUtils.FirstElement(this.SelectedItems);
			if (selectedNote == null)
				return;

			// only allow editing of non-expired notes
			this.ActionModel.Edit.Enabled &= !selectedNote.IsExpired;

			// only allow deletion of new notes
			this.ActionModel.Delete.Enabled &= selectedNote.CreationTime == null;
		}

		protected override IList<PatientNoteDetail> ListItems(int firstRow, int maxRows)
		{
			return _notes;
		}

		protected override bool AddItems(out IList<PatientNoteDetail> addedItems)
		{
			addedItems = new List<PatientNoteDetail>();

			var newNote = new PatientNoteDetail();
			var editor = new PatientNoteEditorComponent(newNote, _noteCategoryChoices);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				addedItems.Add(newNote);
				_notes.Add(newNote);
				return true;
			}

			return false;
		}

		protected override bool EditItems(IList<PatientNoteDetail> items, out IList<PatientNoteDetail> editedItems)
		{
			editedItems = new List<PatientNoteDetail>();

			var originalNote = CollectionUtils.FirstElement(items);
			var editedNote = (PatientNoteDetail) originalNote.Clone();
			var editor = new PatientNoteEditorComponent(editedNote, _noteCategoryChoices);
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

		protected override bool DeleteItems(IList<PatientNoteDetail> items, out IList<PatientNoteDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<PatientNoteDetail>();

			foreach(var item in items)
			{
				deletedItems.Add(item);
				_notes.Remove(item);
			}

			return deletedItems.Count > 0;
		}

		protected override bool IsSameItem(PatientNoteDetail x, PatientNoteDetail y)
		{
			if (ReferenceEquals(x, y))
				return true;

			// if only one is null, they are not the same
			if (x.PatientNoteRef == null || y.PatientNoteRef == null)
				return false;

			return x.PatientNoteRef.Equals(y.PatientNoteRef, true);
		}

		#endregion
	}
}
