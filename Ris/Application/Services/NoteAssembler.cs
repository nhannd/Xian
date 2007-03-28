using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class NoteAssembler
    {
        public NoteDetail CreateNoteDetail(Note note, IPersistenceContext context)
        {
            if (note == null)
                return null;

            return new NoteDetail(note.Text, note.Severity, note.TimeStamp);
        }

        public Note CreateNote(NoteDetail noteDetail)
        {
            if (noteDetail == null)
                return null;

            return new Note(noteDetail.Text, noteDetail.Severity, noteDetail.TimeStamp);
        }

        public void AddNote(NoteDetail noteDetail, IList notes)
        {
            Note newNote = CreateNote(noteDetail);

            notes.Add(newNote);
        }
    }
}
