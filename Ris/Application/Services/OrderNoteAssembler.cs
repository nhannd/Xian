using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderNoteAssembler
    {
        class OrderNoteSynchronizeHelper : SynchronizeHelper<OrderNote, OrderNoteDetail>
        {
            private readonly OrderNoteAssembler _assembler;
            private readonly Staff _currentUserStaff;
            private readonly IPersistenceContext _context;

            public OrderNoteSynchronizeHelper(OrderNoteAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
            {
                _assembler = assembler;
                _currentUserStaff = currentUserStaff;
                _context = context;

                _allowUpdate = false;
                _allowRemove = false;
            }

            protected override bool CompareItems(OrderNote domainItem, OrderNoteDetail sourceItem)
            {
                return Equals(domainItem.CreationTime, sourceItem.CreationTime) &&
                       Equals(domainItem.Author.GetRef(), sourceItem.Author.StaffRef);
            }

            protected override OrderNote CreateDomainItem(OrderNoteDetail sourceItem)
            {
                return _assembler.CreateOrderNote(sourceItem, _currentUserStaff, _context);
            }
        }

        public void Synchronize(IList<OrderNote> domainList, IList<OrderNoteDetail> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            OrderNoteSynchronizeHelper synchronizer = new OrderNoteSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public OrderNote CreateOrderNote(OrderNoteDetail detail, Staff currentStaff, IPersistenceContext context)
        {
            OrderNote newNote = new OrderNote();

            if (detail.Author != null)
                newNote.Author = context.Load<Staff>(detail.Author.StaffRef, EntityLoadFlags.Proxy);
            else
                newNote.Author = currentStaff;

            newNote.Comment = detail.Comment;

            return newNote;
        }

        public OrderNoteDetail CreateOrderNoteDetail(OrderNote note, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteDetail(
                note.CreationTime,
                staffAssembler.CreateStaffSummary(note.Author, context),
                note.Comment);
        }
    }
}
