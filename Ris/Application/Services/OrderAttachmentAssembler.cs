#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderAttachmentAssembler
    {
        class OrderAttachmentSynchronizeHelper : CollectionSynchronizeHelper<OrderAttachment, OrderAttachmentSummary>
        {
            private readonly OrderAttachmentAssembler _assembler;
            private readonly IPersistenceContext _context;
            private readonly Staff _currentUserStaff;

            public OrderAttachmentSynchronizeHelper(OrderAttachmentAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
                : base(true, true)
            {
                _assembler = assembler;
                _context = context;
                _currentUserStaff = currentUserStaff;
            }

            protected override bool CompareItems(OrderAttachment domainItem, OrderAttachmentSummary sourceItem)
            {
                return Equals(domainItem.Document.GetRef(), sourceItem.Document.DocumentRef);
            }

            protected override void AddItem(OrderAttachmentSummary sourceItem, ICollection<OrderAttachment> domainList)
            {
                OrderAttachment attachment = _assembler.CreateOrderAttachment(sourceItem, _currentUserStaff, _context);
                attachment.Document.Attach();
                domainList.Add(attachment);
            }

            protected override void UpdateItem(OrderAttachment domainItem, OrderAttachmentSummary sourceItem, ICollection<OrderAttachment> domainList)
            {
                _assembler.UpdateOrderAttachment(domainItem, sourceItem, _context);
            }

            protected override void RemoveItem(OrderAttachment domainItem, ICollection<OrderAttachment> domainList)
            {
                domainList.Remove(domainItem);
            }
        }

        public void Synchronize(IList<OrderAttachment> domainList, IList<OrderAttachmentSummary> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            OrderAttachmentSynchronizeHelper synchronizer = new OrderAttachmentSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public OrderAttachmentSummary CreateOrderAttachmentSummary(OrderAttachment attachment, IPersistenceContext context)
        {
            AttachedDocumentAssembler attachedDocAssembler = new AttachedDocumentAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();

            return new OrderAttachmentSummary(
                EnumUtils.GetEnumValueInfo(attachment.Category),
                staffAssembler.CreateStaffSummary(attachment.AttachedBy, context),
                attachedDocAssembler.CreateAttachedDocumentSummary(attachment.Document));
        }

        public OrderAttachment CreateOrderAttachment(OrderAttachmentSummary summary, Staff currentUserStaff, IPersistenceContext context)
        {
            return new OrderAttachment(
                EnumUtils.GetEnumValue<OrderAttachmentCategoryEnum>(summary.Category, context),
                summary.AttachedBy == null ? currentUserStaff : context.Load<Staff>(summary.AttachedBy.StaffRef),
                context.Load<AttachedDocument>(summary.Document.DocumentRef));
        }

        public void UpdateOrderAttachment(OrderAttachment attachment, OrderAttachmentSummary summary, IPersistenceContext context)
        {
            AttachedDocumentAssembler mimeDocAssembler = new AttachedDocumentAssembler();
            attachment.Category = EnumUtils.GetEnumValue<OrderAttachmentCategoryEnum>(summary.Category, context);
            mimeDocAssembler.UpdateAttachedDocumentSummary(attachment.Document, summary.Document);
        }
    }
}
