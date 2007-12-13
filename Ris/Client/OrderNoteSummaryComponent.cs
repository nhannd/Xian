using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

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
        private readonly OrderNoteTable _noteTable;
        private readonly CrudActionModel _noteActionHandler;
        private OrderNoteDetail _currentNoteSelection;
        private List<OrderNoteDetail> _notes;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderNoteSummaryComponent()
        {
            _notes = new List<OrderNoteDetail>();
            _noteTable = new OrderNoteTable();

            _noteActionHandler = new CrudActionModel();
            _noteActionHandler.Add.SetClickHandler(AddNote);
            _noteActionHandler.Edit.SetClickHandler(UpdateSelectedNote);
            _noteActionHandler.Delete.SetClickHandler(DeleteSelectedNote);

            _noteActionHandler.Add.Enabled = true;
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
                _noteTable.Items.AddRange(_notes);
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
            OrderNoteDetail note = new OrderNoteDetail("");

            OrderNoteEditorComponent editor = new OrderNoteEditorComponent(note);
            ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddNote);
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                _noteTable.Items.Add(note);
                _notes.Add(note);
                this.Modified = true;
            }
        }

        public void UpdateSelectedNote()
        {
            // can occur if user double clicks while holding control
            if (_currentNoteSelection == null) return;

            OrderNoteDetail note = (OrderNoteDetail)_currentNoteSelection.Clone();

            OrderNoteEditorComponent editor = new OrderNoteEditorComponent(note);
            ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateNote);
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                // delete and re-insert to ensure that TableView updates correctly
                OrderNoteDetail toBeRemoved = _currentNoteSelection;
                _noteTable.Items.Remove(toBeRemoved);
                _notes.Remove(toBeRemoved);

                _noteTable.Items.Add(note);
                _notes.Add(note);

                this.Modified = true;
            }
        }

        public void DeleteSelectedNote()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedNote, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
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
            _noteActionHandler.Edit.Enabled = _currentNoteSelection != null;
            _noteActionHandler.Delete.Enabled = (_currentNoteSelection != null && _currentNoteSelection.CreationTime == null);
        }
    }
}
