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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderNoteAssembler
    {
        class OrderNoteSynchronizeHelper : CollectionSynchronizeHelper<OrderNote, OrderNoteDetail>
        {
            private readonly OrderNoteAssembler _assembler;
            private readonly Staff _currentUserStaff;
            private readonly IPersistenceContext _context;

            public OrderNoteSynchronizeHelper(OrderNoteAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
                :base(false, false)
            {
                _assembler = assembler;
                _currentUserStaff = currentUserStaff;
                _context = context;
            }

            protected override bool CompareItems(OrderNote domainItem, OrderNoteDetail sourceItem)
            {
                return domainItem.GetRef().Equals(sourceItem.OrderNoteRef, true);
            }

            protected override void AddItem(OrderNoteDetail sourceItem, ICollection<OrderNote> notes)
            {
                notes.Add( _assembler.CreateOrderNote(sourceItem, _currentUserStaff, _context));
            }
        }

        public void Synchronize(ICollection<OrderNote> domainList, IList<OrderNoteDetail> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            OrderNoteSynchronizeHelper synchronizer = new OrderNoteSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public OrderNoteDetail CreateOrderNoteDetail(OrderNote note, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();

            //TODO compute recipients
            return new OrderNoteDetail(
                note.GetRef(),
                note.Category,
                note.CreationTime,
                note.SentTime,
                staffAssembler.CreateStaffSummary(note.Sender, context),
                new List<OrderNoteDetail.StaffRecipientDetail>(), 
                new List<OrderNoteDetail.GroupRecipientDetail>(), 
                note.Body);
        }

        public OrderNoteSummary CreateOrderNoteSummary(OrderNote note, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteSummary(
                note.GetRef(),
                note.Category,
                note.CreationTime,
                note.SentTime,
                staffAssembler.CreateStaffSummary(note.Sender, context),
                false,  //TODO compute acknowledgement status
                note.Body);
        }

        public OrderNote CreateOrderNote(OrderNoteDetail detail, Staff currentStaff, IPersistenceContext context)
        {
            OrderNote newNote = new OrderNote();

            if (detail.Author != null)
                newNote.Sender = context.Load<Staff>(detail.Author.StaffRef, EntityLoadFlags.Proxy);
            else
                newNote.Sender = currentStaff;

            newNote.Body = detail.NoteBody;

            return newNote;
        }
    }
}
