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
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestDiagnosticServiceFactory
    {
        internal static DiagnosticService CreateDiagnosticService()
        {
            return CreateDiagnosticService(1);
        }

        internal static DiagnosticService CreateDiagnosticService(int numReqProcs)
        {
            // create a bunch of dummy procedure types (without procedure plans)
            HashedSet<ProcedureType> procedureTypes = new HashedSet<ProcedureType>();
            for (int p = 0; p < numReqProcs; p++)
            {
                ProcedureType pt = new ProcedureType("20" + p, "Procedure 20" + p);
                procedureTypes.Add(pt);
            }
            return new DiagnosticService("301", "Diagnostic Service 301", procedureTypes);
        }
    }
}
