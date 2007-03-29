using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="NoteSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteSummaryComponent class
    /// </summary>
    [AssociateView(typeof(NoteSummaryComponentViewExtensionPoint))]
    public class NoteSummaryComponent : ApplicationComponent
    {
        private List<NoteDetail> _noteList;
        private NoteTable _noteTable;
        private NoteDetail _currentNoteSelection;
        private CrudActionModel _noteActionHandler;
        private List<NoteCategorySummary> _noteCategoryChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteSummaryComponent(List<NoteCategorySummary> categoryChoices)
        {
            _noteCategoryChoices = categoryChoices;
            _noteTable = new NoteTable();

            _noteActionHandler = new CrudActionModel();
            _noteActionHandler.Add.SetClickHandler(AddNote);
            _noteActionHandler.Edit.SetClickHandler(UpdateSelectedNote);
            _noteActionHandler.Delete.SetClickHandler(DeleteSelectedNote);

            _noteActionHandler.Add.Enabled = true;
            _noteActionHandler.Edit.Enabled = false;
            _noteActionHandler.Delete.Enabled = false;
        }

        public List<NoteDetail> Subject
        {
            get { return _noteList; }
            set { _noteList = value; }
        }

        public override void Start()
        {
            _noteTable.Items.AddRange(_noteList);

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable Notes
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
                _currentNoteSelection = (NoteDetail)value.Item;
                NoteSelectionChanged();
            }
        }

        public void AddNote()
        {
            NoteDetail note = new NoteDetail();

            NoteEditorComponent editor = new NoteEditorComponent(note, _noteCategoryChoices);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddNote);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                note.TimeStamp = Platform.Time;

                _noteTable.Items.Add(note);
                _noteList.Add(note);
                this.Modified = true;
            }
        }

        public void UpdateSelectedNote()
        {
            // can occur if user double clicks while holding control
            if (_currentNoteSelection == null) return;

            NoteDetail note = (NoteDetail)_currentNoteSelection.Clone();

            NoteEditorComponent editor = new NoteEditorComponent(note, _noteCategoryChoices);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateNote);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                NoteDetail toBeRemoved = _currentNoteSelection;
                _noteTable.Items.Remove(toBeRemoved);
                _noteList.Remove(toBeRemoved);

                _noteTable.Items.Add(note);
                _noteList.Add(note);

                this.Modified = true;
            }
        }

        public void DeleteSelectedNote()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedNote, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary note otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong note being removed from the Patient
                NoteDetail toBeRemoved = _currentNoteSelection;
                _noteTable.Items.Remove(toBeRemoved);
                _noteList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void NoteSelectionChanged()
        {
            _noteActionHandler.Edit.Enabled =
                _noteActionHandler.Delete.Enabled = (_currentNoteSelection != null);
        }
    }
}
