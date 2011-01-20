#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientNoteAssembler
    {
		public void Synchronize(Patient patient, ICollection<PatientNoteDetail> sourceList, Staff newNoteAuthor, IPersistenceContext context)
        {
			foreach (PatientNoteDetail noteDetail in sourceList)
			{
				if(noteDetail.PatientNoteRef == null)
				{
					patient.AddNote(CreateNote(noteDetail, newNoteAuthor, context));
				}
				else
				{
					PatientNote note = CollectionUtils.SelectFirst(patient.Notes,
						delegate(PatientNote n) { return n.GetRef().Equals(noteDetail.PatientNoteRef, true); });

					if(note != null)
					{
						UpdateNote(note, noteDetail);
					}
				}
			}
        }

        public PatientNoteDetail CreateNoteDetail(PatientNote note, IPersistenceContext context)
        {
            if (note == null)
                return null;

			PatientNoteCategoryAssembler categoryAssembler = new PatientNoteCategoryAssembler();
			StaffAssembler staffAssembler = new StaffAssembler();

			return new PatientNoteDetail(
        		note.GetRef(),
        		note.Comment,
        		categoryAssembler.CreateNoteCategorySummary(note.Category, context),
        		staffAssembler.CreateStaffSummary(note.Author, context),
        		note.CreationTime,
        		note.ValidRange.From,
        		note.ValidRange.Until,
        		note.IsExpired);
        }

        public PatientNote CreateNote(PatientNoteDetail detail, Staff author, IPersistenceContext context)
        {
            PatientNoteCategory category = context.Load<PatientNoteCategory>(detail.Category.NoteCategoryRef, EntityLoadFlags.Proxy);
            PatientNote note = new PatientNote(author, category, detail.Comment);
            note.ValidRange.Until = detail.ValidRangeUntil;

            return note;
        }

        public void UpdateNote(PatientNote note, PatientNoteDetail detail)
        {
            // the only properties of the note that can be updated is the ValidRange
            // and only if it is not already expired
            if (!note.IsExpired)
                note.ValidRange.Until = detail.ValidRangeUntil;
        }
    }
}
