#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestFacilityFactory
    {
        internal static Facility CreateFacility()
        {
            return new Facility(
				"TCH",
				"Toronto Community Hospital",
				null,
				new InformationAuthorityEnum("UHN", "UHN", "University Health Network"),
				new HashedSet<Department>());
        }
    }
}
