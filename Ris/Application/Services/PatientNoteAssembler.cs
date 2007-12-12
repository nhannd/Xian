#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientNoteAssembler
    {
        class PatientNoteSynchronizeHelper : SynchronizeHelper<PatientNote, PatientNoteDetail>
        {
            private PatientNoteAssembler _assembler;
            private IPersistenceContext _context;

            public PatientNoteSynchronizeHelper(PatientNoteAssembler assembler, IPersistenceContext context)
            {
                _assembler = assembler;
                _context = context;
            }

            protected override bool CompareItems(PatientNote domainItem, PatientNoteDetail sourceItem)
            {
                return Equals(domainItem.CreationTime, sourceItem.CreationTime) &&
                       Equals(domainItem.Author.GetRef(), sourceItem.Author.StaffRef);
            }

            protected override PatientNote CreateDomainItem(PatientNoteDetail sourceItem)
            {
                return _assembler.CreateNote(sourceItem, _context);
            }

            protected override void UpdateDomainItem(PatientNote domainItem, PatientNoteDetail sourceItem)
            {
                _assembler.UpdateNote(domainItem, sourceItem, _context);
            }
        }

        public void Synchronize(IList<PatientNote> domainList, IList<PatientNoteDetail> sourceList, IPersistenceContext context)
        {
            PatientNoteSynchronizeHelper synchronizer = new PatientNoteSynchronizeHelper(this, context);
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

            PatientNoteCategoryAssembler categoryAssembler = new PatientNoteCategoryAssembler();
            detail.Category = categoryAssembler.CreateNoteCategorySummary(note.Category, context);

            StaffAssembler staffAssembler = new StaffAssembler();
            detail.Author = staffAssembler.CreateStaffSummary(note.Author, context);

            return detail;
        }

        public PatientNote CreateNote(PatientNoteDetail detail, IPersistenceContext context)
        {
            PatientNote newNote = new PatientNote();

            newNote.Comment = detail.Comment;
            newNote.ValidRange.From = detail.ValidRangeFrom;
            newNote.ValidRange.Until = detail.ValidRangeUntil;

            if (detail.CreationTime != null)
                newNote.CreationTime = detail.CreationTime.Value;
            else
                newNote.CreationTime = Platform.Time;

            if (detail.Category != null)
                newNote.Category = context.Load<PatientNoteCategory>(detail.Category.NoteCategoryRef, EntityLoadFlags.Proxy);

            if (detail.Author != null)
                newNote.Author = context.Load<Staff>(detail.Author.StaffRef, EntityLoadFlags.Proxy);
            else
            {
                //TODO: Services should know which staff is invoking the operation, use that staff instead
                newNote.Author = context.GetBroker<IStaffBroker>().FindOne(new StaffSearchCriteria());
            }

            return newNote;
        }

        public void UpdateNote(PatientNote note, PatientNoteDetail detail, IPersistenceContext context)
        {
            // The creation time and author should never be changed
            note.Comment = detail.Comment;
            note.ValidRange.From = detail.ValidRangeFrom;
            note.ValidRange.Until = detail.ValidRangeUntil;
            if (detail.Category != null)
                note.Category = context.Load<PatientNoteCategory>(detail.Category.NoteCategoryRef, EntityLoadFlags.Proxy);
        }
    }
}
