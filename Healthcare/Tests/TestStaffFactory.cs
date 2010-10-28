#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Tests
{
	internal static class TestStaffFactory
	{
		internal static Staff CreateStaff()
		{
			return CreateStaff(new StaffTypeEnum("any", "any", "any"));
		}

		internal static Staff CreateStaff(StaffTypeEnum staffType)
		{
			return new Staff(
				"01",
				new PersonName("Simpson", "Bart", null, null, null, null),
				Sex.M,
				null,   // title
				null,   // license
				null,   // billing
				staffType,
				null,
				new List<EmailAddress>(),
				new List<Address>(),
				new List<TelephoneNumber>(),
				new Dictionary<string, string>(),
				new HashedSet<StaffGroup>());
		}
	}
}
