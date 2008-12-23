#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="OrderNoteSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class OrderNoteSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// OrderNoteSummaryComponent class
    /// </summary>
    [AssociateView(typeof(OrderNoteSummaryComponentViewExtensionPoint))]
    public class OrderNoteSummaryComponent : ApplicationComponent
    {
        private readonly OrderNoteCategory _category;
        private readonly OrderNoteTable _noteTable;
        private readonly CrudActionModel _noteActionHandler;
        private OrderNoteDetail _currentNoteSelection;
        private List<OrderNoteDetail> _notes;

        /// <summary>
        /// Constructor allowing edits.
        /// </summary>
        /// <param name="category"></param>
        public OrderNoteSummaryComponent(OrderNoteCategory category)
            : this(category, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category">Specifies the category of order notes to display, and the category under which new notes are placed.</param>
        /// <param name="canEdit">Specifies if the component is editable.  If not, all action buttons are disabled.</param>
        public OrderNoteSummaryComponent(OrderNoteCategory category, bool canEdit)
        {
            Platform.CheckForNullReference(category, "category");

            _category = category;
            _notes = new List<OrderNoteDetail>();
            _noteTable = new OrderNoteTable(this);

            _noteActionHandler = new CrudActionModel();
            _noteActionHandler.Add.SetClickHandler(AddNote);
            _noteActionHandler.Edit.SetClickHandler(UpdateSelectedNote);
            _noteActionHandler.Delete.SetClickHandler(DeleteSelectedNote);

            // only need to initialize the add button, since the edit and delete buttons are only ever enabled
            // for newly added notes.  See NoteSelectionChanged()
            _noteActionHandler.Add.Enabled = canEdit;
            _noteActionHandler.Edit.Enabled = false;
            _noteActionHandler.Delete.Enabled = false;
        }

        public List<OrderNoteDetail> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                _noteTable.Items.Clear();
                _noteTable.Items.AddRange(
                    CollectionUtils.Select(value,
                        delegate(OrderNoteDetail d) { return d.Category == _category.Key; }));
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
                _currentNoteSelection = (OrderNoteDetail)value.Item;
                NoteSelectionChanged();
            }
        }

        public void AddNote()
        {
            OrderNoteDetail note = new OrderNoteDetail(_category.Key, "", null, false, null, null);

            try
            {
                OrderNoteEditorComponent editor = new OrderNoteEditorComponent(note);
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

            OrderNoteDetail notedetail;

            // manually clone order note
            notedetail = new OrderNoteDetail(
                _currentNoteSelection.OrderNoteRef,
                _currentNoteSelection.Category,
                _currentNoteSelection.CreationTime,
                _currentNoteSelection.PostTime,
                _currentNoteSelection.Author,
                _currentNoteSelection.OnBehalfOfGroup,
                _currentNoteSelection.Urgent,
                _currentNoteSelection.StaffRecipients,
                _currentNoteSelection.GroupRecipients,
                _currentNoteSelection.NoteBody,
                _currentNoteSelection.CanAcknowledge
                );

            UpdateNoteDetail(notedetail);
        }

        public void UpdateNoteDetail(OrderNoteDetail notedetail)
        {
            try
            {
                OrderNoteEditorComponent editor = new OrderNoteEditorComponent(notedetail);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // delete and re-insert to ensure that TableView updates correctly
                    OrderNoteDetail toBeRemoved = _currentNoteSelection;
                    _noteTable.Items.Remove(toBeRemoved);
                    _notes.Remove(toBeRemoved);

                    _noteTable.Items.Add(notedetail);
                    _notes.Add(notedetail);

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
            if (this.Host.ShowMessageBox(SR.MessageConfirmDeleteSelectedNote, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary note otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong note being removed from the Patient
                OrderNoteDetail toBeRemoved = _currentNoteSelection;
                _noteTable.Items.Remove(toBeRemoved);
                _notes.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void NoteSelectionChanged()
        {
            // only un-posted notes can be edited or deleted
            _noteActionHandler.Edit.Enabled = _noteActionHandler.Delete.Enabled =
                (_currentNoteSelection != null && _currentNoteSelection.PostTime == null);
        }
    }
}
