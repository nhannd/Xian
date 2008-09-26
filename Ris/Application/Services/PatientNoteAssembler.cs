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
