#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		private readonly IList<PatientNoteDetail> _notes;

		public PatientNoteSummaryComponent(IList<PatientNoteDetail> notes, List<PatientNoteCategorySummary> categoryChoices)
			: base(false)
		{
			_notes = notes;
			_noteCategoryChoices = categoryChoices;
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
			this.ActionModel.Edit.Enabled = !selectedNote.IsExpired;

			// only allow deletion of new notes
			this.ActionModel.Delete.Enabled = selectedNote.CreationTime == null;
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

			var note = CollectionUtils.FirstElement(items);
			var editor = new PatientNoteEditorComponent(note, _noteCategoryChoices);
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText))
			{
				editedItems.Add(note);
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
