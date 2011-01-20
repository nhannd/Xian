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
using Iesi.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestExternalPractitionerFactory
    {
        internal static ExternalPractitioner CreatePractitioner()
        {
            return new ExternalPractitioner(
                new PersonName("Who", "Doctor", null, null, null, null),
                "1234",
                "5678",
				false,
				null,
				Platform.Time,
                new HashedSet<ExternalPractitionerContactPoint>(),
				new Dictionary<string, string>());
        }
    }
}
