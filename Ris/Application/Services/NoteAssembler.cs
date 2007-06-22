using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
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
            newNote.ValidRange.From = detail.ValidRangeFrom;
            newNote.ValidRange.Until = detail.ValidRangeUntil;

            if (detail.TimeStamp != null)
                newNote.TimeStamp = detail.TimeStamp.Value;
            else
                newNote.TimeStamp = Platform.Time;

            if (detail.Category != null)
                newNote.Category = context.Load<NoteCategory>(detail.Category.NoteCategoryRef, EntityLoadFlags.Proxy);

            if (detail.CreatedBy != null)
                newNote.CreatedBy = context.Load<Staff>(detail.CreatedBy.StaffRef, EntityLoadFlags.Proxy);
            else
            {
                //TODO: Services should know which staff is invoking the operation, use that staff instead
                newNote.CreatedBy = context.GetBroker<IStaffBroker>().FindOne(new StaffSearchCriteria());
            }

            return newNote;
        }
    }
}
