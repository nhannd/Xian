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
            private readonly Order _order;
            private readonly OrderNoteAssembler _assembler;
            private readonly Staff _newNoteAuthor;
            private readonly IPersistenceContext _context;

            public OrderNoteSynchronizeHelper(OrderNoteAssembler assembler, Order order, Staff newNoteAuthor, IPersistenceContext context)
                :base(false, false)
            {
                _assembler = assembler;
                _order = order;
                _newNoteAuthor = newNoteAuthor;
                _context = context;
            }

            protected override bool CompareItems(OrderNote domainItem, OrderNoteDetail sourceItem)
            {
                return domainItem.GetRef().Equals(sourceItem.OrderNoteRef, true);
            }

            protected override void AddItem(OrderNoteDetail sourceItem, ICollection<OrderNote> notes)
            {
                notes.Add( _assembler.CreateOrderNote(sourceItem, _order, _newNoteAuthor, true, _context));
            }
        }

        /// <summary>
        /// Synchronizes an order's Notes collection, adding any notes that don't already exist in the collection,
        /// using the specified author.  The notes are posted immediately.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="sourceList"></param>
        /// <param name="newNoteAuthor"></param>
        /// <param name="context"></param>
        public void SynchronizeOrderNotes(Order order, IList<OrderNoteDetail> sourceList, Staff newNoteAuthor, IPersistenceContext context)
        {
            OrderNoteSynchronizeHelper synchronizer = new OrderNoteSynchronizeHelper(this, order, newNoteAuthor, context);
            synchronizer.Synchronize(order.Notes, sourceList);
        }

        public OrderNoteDetail CreateOrderNoteDetail(OrderNote note, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();

            List<OrderNoteDetail.StaffRecipientDetail> staffRecipients = new List<OrderNoteDetail.StaffRecipientDetail>();
            List<OrderNoteDetail.GroupRecipientDetail> groupRecipients = new List<OrderNoteDetail.GroupRecipientDetail>();
            if (note.IsPosted)
            {
                foreach (NoteReadActivity readActivity in note.ReadActivities)
                {
                    if(readActivity.Recipient.Group != null)
                    {
                        groupRecipients.Add(
                            new OrderNoteDetail.GroupRecipientDetail(
                                staffGroupAssembler.CreateSummary(readActivity.Recipient.Group),
                                readActivity.IsAcknowledged,
                                readActivity.AcknowledgedBy.Time));
                    }
                    else
                    {
                        staffRecipients.Add(
                            new OrderNoteDetail.StaffRecipientDetail(
                                staffAssembler.CreateStaffSummary(readActivity.Recipient.Staff, context),
                                readActivity.IsAcknowledged,
                                readActivity.AcknowledgedBy.Time));
                    }
                }
            }
            else
            {
                foreach (NoteRecipient recipient in note.Recipients)
                {
                    if(recipient.Group != null)
                    {
                        groupRecipients.Add(
                            new OrderNoteDetail.GroupRecipientDetail(
                                staffGroupAssembler.CreateSummary(recipient.Group),
                                false,
                                null));
                    }
                    else
                    {
                        staffRecipients.Add(
                            new OrderNoteDetail.StaffRecipientDetail(
                                staffAssembler.CreateStaffSummary(recipient.Staff, context),
                                false,
                                null));
                    }
                }    
            }

            return new OrderNoteDetail(
                note.GetRef(),
                note.Category,
                note.CreationTime,
                note.PostTime,
                staffAssembler.CreateStaffSummary(note.Author, context),
                staffRecipients, 
                groupRecipients, 
                note.Body);
        }

        public OrderNoteSummary CreateOrderNoteSummary(OrderNote note, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteSummary(
                note.GetRef(),
                note.Category,
                note.CreationTime,
                note.PostTime,
                staffAssembler.CreateStaffSummary(note.Author, context),
                note.IsFullyAcknowledged,
                note.Body);
        }

        public OrderNote CreateOrderNote(OrderNoteDetail detail, Order order, Staff author, bool post, IPersistenceContext context)
        {
            return new OrderNote(order, detail.Category, author, detail.NoteBody, post);
        }

    }
}
