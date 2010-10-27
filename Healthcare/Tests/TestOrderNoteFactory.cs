#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
	internal static class TestOrderNoteFactory
	{
		internal static IList<OrderNote> CreateOrderNotes()
		{
			IList<OrderNote> notes = new List<OrderNote>();

			notes.Add(CreateOrderNote("Test note"));

			return notes;
		}

		internal static OrderNote CreateOrderNote(string comment)
		{
			//return new OrderNote(Platform.Time, TestStaffFactory.CreateStaff(StaffType.STEC), comment);
			OrderNote note = new OrderNote();
			note.CreationTime = Platform.Time;
			note.Author = TestStaffFactory.CreateStaff(new StaffTypeEnum("STEC", null, null));
			note.Body = comment;
			return note;
		}
	}
}
