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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientNoteSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientNoteSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientNoteSummaryComponent class
    /// </summary>
    [AssociateView(typeof(PatientNoteSummaryComponentViewExtensionPoint))]
    public class PatientNoteSummaryComponent : ApplicationComponent
    {
        private readonly List<PatientNoteCategorySummary> _noteCategoryChoices;
        private readonly CrudActionModel _noteActionHandler;
        private readonly PatientNoteTable _noteTable;
        private List<PatientNoteDetail> _notes;
        private PatientNoteDetail _currentNoteSelection;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientNoteSummaryComponent(List<PatientNoteCategorySummary> categoryChoices)
        {
            _noteCategoryChoices = categoryChoices;
            _noteTable = new PatientNoteTable();

            _noteActionHandler = new CrudActionModel();
            _noteActionHandler.Add.SetClickHandler(AddNote);
            _noteActionHandler.Edit.SetClickHandler(UpdateSelectedNote);
            _noteActionHandler.Delete.SetClickHandler(DeleteSelectedNote);

            _noteActionHandler.Add.Enabled = true;
            _noteActionHandler.Edit.Enabled = false;
            _noteActionHandler.Delete.Enabled = false;
        }

        public List<PatientNoteDetail> Subject
        {
            get { return _notes; }
            set
            {
                _notes = value;
                _noteTable.Items.Clear();
                _noteTable.Items.AddRange(_notes);
                _noteTable.Sort();
            }
        }

        #region Presentation Model

        public ITable NoteTable
        {
            get { return _noteTable; }
        }

        public ActionModelNode NoteActionModel
        {
            get { return _noteActionHandler; }
        }

        public ISelection SelectedNote
        {
            get { return new Selection(_currentNoteSelection); }
            set
            {
                _currentNoteSelection = (PatientNoteDetail)value.Item;
                UpdateNoteActionEnablement();
            }
        }

        public void AddNote()
        {
            PatientNoteDetail note = new PatientNoteDetail();

            try
            {
                PatientNoteEditorComponent editor = new PatientNoteEditorComponent(note, _noteCategoryChoices);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _noteTable.Items.Add(note);
                    _notes.Add(note);
                    this.Modified = true;
                }

            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedNote()
        {
            // can occur if user double clicks while holding control
            if (_currentNoteSelection == null) return;

            // don't allow editing of expired notes
            if(_currentNoteSelection.IsExpired)
                return;

            PatientNoteDetail note = (PatientNoteDetail)_currentNoteSelection.Clone();

            try
            {
                PatientNoteEditorComponent editor = new PatientNoteEditorComponent(note, _noteCategoryChoices);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _notes.Remove(_currentNoteSelection);
                    _notes.Add(note);

                    // update table
                    int i = _noteTable.Items.IndexOf(_currentNoteSelection);
                    _noteTable.Items[i] = note;

                    _currentNoteSelection = note;
                    NotifyPropertyChanged("SelectedNote");

                    this.Modified = true;
                }

            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DeleteSelectedNote()
        {
            if(_currentNoteSelection == null || !IsNewNote(_currentNoteSelection))
                return;

            if (this.Host.ShowMessageBox(SR.MessageConfirmDeleteSelectedNote, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary note otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong note being removed from the Patient
                PatientNoteDetail toBeRemoved = _currentNoteSelection;
                _noteTable.Items.Remove(toBeRemoved);
                _notes.Remove(toBeRemoved);
            }
        }

        #endregion

        private void UpdateNoteActionEnablement()
        {
            // don't allow editing of expired notes
            _noteActionHandler.Edit.Enabled = _currentNoteSelection != null && !_currentNoteSelection.IsExpired;
            _noteActionHandler.Delete.Enabled = (_currentNoteSelection != null) && IsNewNote(_currentNoteSelection);
        }

        private bool IsNewNote(PatientNoteDetail note)
        {
            return note.CreationTime == null;
        }
    }
}
