using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestStaffFactory
    {
        internal static Staff CreateStaff(StaffType staffType)
        {
            return new Staff(
                "01",
                new PersonName("Simpson", "Bart", null, null, null, null),
                staffType,
                null);
        }
    }
}
