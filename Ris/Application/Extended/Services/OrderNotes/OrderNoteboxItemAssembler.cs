#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.OrderNotes
{
	class OrderNoteboxItemAssembler
	{
		public OrderNoteboxItemSummary CreateSummary(OrderNoteboxItem item, IPersistenceContext context)
		{
			var mrnAssembler = new MrnAssembler();
			var nameAssembler = new PersonNameAssembler();
			var staffAssembler = new StaffAssembler();
			var groupAssembler = new StaffGroupAssembler();

			var staffRecipients = new List<StaffSummary>();
			var groupRecipients = new List<StaffGroupSummary>();

			foreach (var recipient in item.Recipients)
			{
				if (recipient is Staff)
					staffRecipients.Add(staffAssembler.CreateStaffSummary((Staff)recipient, context));
				if (recipient is StaffGroup)
					groupRecipients.Add(groupAssembler.CreateSummary((StaffGroup)recipient));
			}

			return new OrderNoteboxItemSummary(
				item.NoteRef,
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
