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
        class PatientNoteSynchronizeHelper : CollectionSynchronizeHelper<PatientNote, PatientNoteDetail>
        {
            private readonly PatientNoteAssembler _assembler;
            private readonly Staff _currentUserStaff;
            private readonly IPersistenceContext _context;

            public PatientNoteSynchronizeHelper(PatientNoteAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
                :base(true, false)
            {
                _assembler = assembler;
                _currentUserStaff = currentUserStaff;
                _context = context;
            }

            protected override bool CompareItems(PatientNote domainItem, PatientNoteDetail sourceItem)
            {
                return Equals(domainItem.CreationTime, sourceItem.CreationTime) &&
                       Equals(domainItem.Author.GetRef(), sourceItem.Author.StaffRef);
            }

            protected override void AddItem(PatientNoteDetail sourceItem, ICollection<PatientNote> notes)
            {
                notes.Add(_assembler.CreateNote(sourceItem, _currentUserStaff, _context));
            }

            protected override void UpdateItem(PatientNote destItem, PatientNoteDetail sourceItem, ICollection<PatientNote> dest)
            {
                _assembler.UpdateNote(destItem, sourceItem);
            }
        }

        public void Synchronize(IList<PatientNote> domainList, IList<PatientNoteDetail> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            PatientNoteSynchronizeHelper synchronizer = new PatientNoteSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public PatientNoteDetail CreateNoteDetail(PatientNote note, IPersistenceContext context)
        {
            if (note == null)
                return null;

            PatientNoteDetail detail = new PatientNoteDetail();

            detail.Comment = note.Comment;
            detail.CreationTime = note.CreationTime;
            detail.ValidRangeFrom = note.ValidRange.From;
            detail.ValidRangeUntil = note.ValidRange.Until;
            detail.IsExpired = note.IsExpired;

            PatientNoteCategoryAssembler categoryAssembler = new PatientNoteCategoryAssembler();
            detail.Category = categoryAssembler.CreateNoteCategorySummary(note.Category, context);

            StaffAssembler staffAssembler = new StaffAssembler();
            detail.Author = staffAssembler.CreateStaffSummary(note.Author, context);

            return detail;
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
