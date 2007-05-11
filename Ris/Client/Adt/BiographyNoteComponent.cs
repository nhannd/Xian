using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
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
        private List<NoteDetail> _noteList;
        private BiographyNoteTable _noteTable;
        private NoteDetail _selectedNote;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyNoteComponent(List<NoteDetail> notes)
        {
            _noteTable = new BiographyNoteTable();
            _noteList = notes;
        }

        public override void Start()
        {
            base.Start();

            _noteTable.Items.AddRange(_noteList);
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ITable Notes
        {
            get { return _noteTable; }
        }

        public ISelection SelectedNote
        {
            get { return new Selection(_selectedNote); }
            set
            {
                _selectedNote = (NoteDetail)value.Item;
                NoteSelectionChanged();
            }
        }

        private void NoteSelectionChanged()
        {
            NotifyAllPropertiesChanged();
        }
    }
}
