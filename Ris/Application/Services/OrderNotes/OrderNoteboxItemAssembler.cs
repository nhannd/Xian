using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.OrderNotes
{
	class OrderNoteboxItemAssembler
	{
		public OrderNoteboxItemSummary CreateSummary(OrderNoteboxItem item, IPersistenceContext context)
		{
			MrnAssembler mrnAssembler = new MrnAssembler();
			PersonNameAssembler nameAssembler = new PersonNameAssembler();
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			List<StaffSummary> staffRecipients = new List<StaffSummary>();
			List<StaffGroupSummary> groupRecipients = new List<StaffGroupSummary>();

			foreach (NoteRecipient recipient in item.Recipients)
			{
				if (recipient.Staff != null)
					staffRecipients.Add(staffAssembler.CreateStaffSummary(recipient.Staff, context));
				if (recipient.Group != null)
					groupRecipients.Add(groupAssembler.CreateSummary(recipient.Group));
			}

			return new OrderNoteboxItemSummary(
				item.OrderNoteRef,
				item.OrderRef,
				item.PatientRef,
				item.PatientProfileRef,
				mrnAssembler.CreateMrnDetail(item.Mrn),
				nameAssembler.CreatePersonNameDetail(item.PatientName),
				item.DateOfBirth,
				item.AccessionNumber,
				item.DiagnosticServiceName,
				item.Category,
				item.PostTime,
				staffAssembler.CreateStaffSummary(item.Author, context),
				item.IsAcknowledged,
				staffRecipients,
				groupRecipients);
		}
	}
}
