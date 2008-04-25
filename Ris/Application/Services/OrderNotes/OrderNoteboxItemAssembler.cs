using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.OrderNotes
{
    class OrderNoteboxItemAssembler
    {
        public OrderNoteboxItemSummary CreateSummary(OrderNoteboxItem item, IPersistenceContext context)
        {
            MrnAssembler mrnAssembler = new MrnAssembler();
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();
            return new OrderNoteboxItemSummary(
                item.OrderNoteRef,
                item.OrderRef,
                item.PatientRef,
                mrnAssembler.CreateMrnDetail(item.Mrn),
                nameAssembler.CreatePersonNameDetail(item.PatientName),
                item.DateOfBirth,
                item.AccessionNumber,
                item.DiagnosticServiceName,
                item.Category,
                item.PostTime,
                staffAssembler.CreateStaffSummary(item.Author, context),
                item.IsAcknowledged);
        }
    }
}
