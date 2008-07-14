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

			foreach (object recipient in item.Recipients)
			{
				if (recipient is Staff)
					staffRecipients.Add(staffAssembler.CreateStaffSummary((Staff)recipient, context));
				if (recipient is StaffGroup)
					groupRecipients.Add(groupAssembler.CreateSummary((StaffGroup)recipient));
			}

			return new OrderNoteboxItemSummary(
				item.OrderRef,
				item.PatientRef,
				item.PatientProfileRef,
				mrnAssembler.CreateMrnDetail(item.Mrn),
				nameAssembler.CreatePersonNameDetail(item.PatientName),
				item.DateOfBirth,
				item.AccessionNumber,
				item.DiagnosticServiceName,
				item.Category,
				item.Urgent,
				item.PostTime,
				staffAssembler.CreateStaffSummary(item.Author, context),
				item.OnBehalfOfGroup == null ? null : groupAssembler.CreateSummary(item.OnBehalfOfGroup),
				item.IsAcknowledged,
				staffRecipients,
				groupRecipients);
		}
	}
}
