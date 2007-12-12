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
