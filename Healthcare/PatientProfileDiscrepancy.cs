using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    [Flags]
    public enum PatientProfileDiscrepancy : uint
    {
        Healthcard      = 0x00000001,
        FamilyName      = 0x00000002,
        GivenName       = 0x00000004,
        DateOfBirth     = 0x00000008,
        Sex             = 0x00000010,
        HomePhone       = 0x00000020,
        HomeAddress     = 0x00000040,
        WorkPhone       = 0x00000080,
        WorkAddress     = 0x00000100,

        All             = 0xffffffff
    }
}
