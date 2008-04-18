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
        #region OrderNoteSynchronizeHelper

        class OrderNoteSynchronizeHelper : CollectionSynchronizeHelper<OrderNote, OrderNoteDetail>
        {
            private readonly Order _order;
            private readonly OrderNoteAssembler _assembler;
            private readonly Staff _newNoteAuthor;
            private readonly IPersistenceContext _context;

            public OrderNoteSynchronizeHelper(OrderNoteAssembler assembler, Order order, Staff newNoteAuthor, IPersistenceContext context)
                : base(false, false)
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
                notes.Add(_assembler.CreateOrderNote(sourceItem, _order, _newNoteAuthor, true, _context));
            }
        }

        #endregion

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

        /// <summary>
        /// Creates an <see cref="OrderNoteDetail"/> from a <see cref="OrderNote"/>.
        /// </summary>
        /// <param name="orderNote"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNoteDetail CreateOrderNoteDetail(OrderNote orderNote, IPersistenceContext context)
        {
            List<OrderNoteDetail.StaffRecipientDetail> staffRecipients = new List<OrderNoteDetail.StaffRecipientDetail>();
            List<OrderNoteDetail.GroupRecipientDetail> groupRecipients = new List<OrderNoteDetail.GroupRecipientDetail>();

            // if the note has been posted, construct the recipients list from the ReadActivites, so we can get the ACK status
            if (orderNote.IsPosted)
            {
                foreach (NoteReadActivity readActivity in orderNote.ReadActivities)
                {
                    if(readActivity.Recipient.IsGroupRecipient)
                    {
                        groupRecipients.Add(
                            CreateGroupRecipientDetail(readActivity.Recipient.Group,
                                                       readActivity.IsAcknowledged,
                                                       readActivity.AcknowledgedBy, context));
                    }
                    else
                    {
                        staffRecipients.Add(
                            CreateStaffRecipientDetail(readActivity.Recipient.Staff,
                                                       readActivity.IsAcknowledged,
                                                       readActivity.AcknowledgedBy, context));
                    }
                }
            }
            else
            {
                // the note has not been posted, so use the Recipients collection
                foreach (NoteRecipient recipient in orderNote.Recipients)
                {
                    if(recipient.Group != null)
                        groupRecipients.Add(CreateGroupRecipientDetail(recipient.Group, false, null, context));
                    else
                        staffRecipients.Add(CreateStaffRecipientDetail(recipient.Staff, false, null, context));
                }    
            }

            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteDetail(
                orderNote.GetRef(),
                orderNote.Category,
                orderNote.CreationTime,
                orderNote.PostTime,
                staffAssembler.CreateStaffSummary(orderNote.Author, context),
                staffRecipients, 
                groupRecipients, 
                orderNote.Body);
        }

        /// <summary>
        /// Creates an <see cref="OrderNoteSummary"/> from a <see cref="OrderNote"/>.
        /// </summary>
        /// <param name="orderNote"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNoteSummary CreateOrderNoteSummary(OrderNote orderNote, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteSummary(
                orderNote.GetRef(),
                orderNote.Category,
                orderNote.CreationTime,
                orderNote.PostTime,
                staffAssembler.CreateStaffSummary(orderNote.Author, context),
                orderNote.IsFullyAcknowledged,
                orderNote.Body);
        }

        /// <summary>
        /// Creates a new <see cref="OrderNote"/> based on the information provided in the specified detail object.
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="order"></param>
        /// <param name="author"></param>
        /// <param name="post"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public OrderNote CreateOrderNote(OrderNoteDetail detail, Order order, Staff author, bool post, IPersistenceContext context)
        {
            List<NoteRecipient> recipients = new List<NoteRecipient>();
            recipients.AddRange(
                CollectionUtils.Map<OrderNoteDetail.StaffRecipientDetail, NoteRecipient>(detail.StaffRecipients,
                    delegate(OrderNoteDetail.StaffRecipientDetail item)
                    {
                        return new NoteRecipient(context.Load<Staff>(item.Staff.StaffRef, EntityLoadFlags.Proxy));
                    }));

            recipients.AddRange(
                CollectionUtils.Map<OrderNoteDetail.GroupRecipientDetail, NoteRecipient>(detail.GroupRecipients,
                    delegate(OrderNoteDetail.GroupRecipientDetail item)
                    {
                        return new NoteRecipient(context.Load<StaffGroup>(item.Group.StaffGroupRef, EntityLoadFlags.Proxy));
                    }));

            return new OrderNote(order, detail.Category, author, detail.NoteBody, recipients, post);
        }

        #region Helpers

        private OrderNoteDetail.GroupRecipientDetail CreateGroupRecipientDetail(StaffGroup group, bool acknowledged,
            NoteReader acknowledgement, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();
            return new OrderNoteDetail.GroupRecipientDetail(
                                staffGroupAssembler.CreateSummary(group),
                                acknowledged,
                                acknowledged ? acknowledgement.Time : null,
                                acknowledged ? staffAssembler.CreateStaffSummary(acknowledgement.Staff, context) : null);
        }

        private OrderNoteDetail.StaffRecipientDetail CreateStaffRecipientDetail(Staff staff, bool acknowledged,
            NoteReader acknowledgement, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteDetail.StaffRecipientDetail(
                                staffAssembler.CreateStaffSummary(staff, context),
                                acknowledged,
                                acknowledged ? acknowledgement.Time : null);
        }

        #endregion

    }
}
