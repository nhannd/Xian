using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class NoteAssembler
    {
        public NoteDetail CreateNoteDetail(Note note, IPersistenceContext context)
        {
            if (note == null)
                return null;

            NoteDetail detail = new NoteDetail();

            detail.Comment = note.Comment;
            detail.TimeStamp = note.TimeStamp;
            detail.ValidRangeFrom = note.ValidRange.From;
            detail.ValidRangeUntil = note.ValidRange.Until;

            NoteCategoryAssembler categoryAssembler = new NoteCategoryAssembler();
            detail.Category = categoryAssembler.CreateNoteCategorySummary(note.Category, context);

            StaffAssembler staffAssembler = new StaffAssembler();
            detail.CreatedBy = staffAssembler.CreateStaffSummary(note.CreatedBy);

            return detail;
        }

        public Note CreateNote(NoteDetail detail, IPersistenceContext context)
        {
            Note newNote = new Note();

            newNote.Comment = detail.Comment;
            newNote.TimeStamp = detail.TimeStamp.Value;
            newNote.ValidRange.From = detail.ValidRangeFrom;
            newNote.ValidRange.Until = detail.ValidRangeUntil;

            if (detail.Category != null)
                newNote.Category = (NoteCategory)context.Load(detail.Category.NoteCategoryRef, EntityLoadFlags.Proxy);

            StaffAssembler staffAssembler = new StaffAssembler();
            detail.CreatedBy = staffAssembler.CreateStaffSummary(newNote.CreatedBy);

            return newNote;
        }
    }
}
