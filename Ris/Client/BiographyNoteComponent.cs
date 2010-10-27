#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyNoteComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyNoteComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// BiographyNoteComponent class
	/// </summary>
	[AssociateView(typeof(BiographyNoteComponentViewExtensionPoint))]
	public class BiographyNoteComponent : ApplicationComponent
	{
		private List<PatientNoteDetail> _noteList;
		private readonly BiographyNoteTable _noteTable;
		private PatientNoteDetail _selectedNote;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyNoteComponent()
		{
			_noteTable = new BiographyNoteTable(this);
		}

		public List<PatientNoteDetail> Notes
		{
			get { return _noteList; }
			set
			{
				_noteList = value;
				_noteTable.Items.Clear();
				_noteTable.Items.AddRange(value);
			}
		}

		public ITable NoteTable
		{
			get { return _noteTable; }
		}

		public ISelection SelectedNote
		{
			get { return new Selection(_selectedNote); }
			set
			{
				_selectedNote = (PatientNoteDetail)value.Item;
				NoteSelectionChanged();
			}
		}

		public void ShowNoteDetail(PatientNoteDetail notedetail)
		{
			try
			{
				var caegotryChoices = new List<PatientNoteCategorySummary> {notedetail.Category};
				var editor = new PatientNoteEditorComponent(notedetail, caegotryChoices, true);
				LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText);
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void NoteSelectionChanged()
		{
			NotifyAllPropertiesChanged();
		}
	}
}
