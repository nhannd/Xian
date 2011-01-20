#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    [Flags]
    public enum PatientProfileDiscrepancy : uint
    {
        None            = 0x00000000,

        Healthcard      = 0x00000001,
        FamilyName      = 0x00000002,
        GivenName       = 0x00000004,
        DateOfBirth     = 0x00000008,
        Sex             = 0x00000010,
        HomePhone       = 0x00000020,
        HomeAddress     = 0x00000040,
        WorkPhone       = 0x00000080,
        WorkAddress     = 0x00000100,
        MiddleName      = 0x00000200,

        All             = 0xffffffff
    }
}
