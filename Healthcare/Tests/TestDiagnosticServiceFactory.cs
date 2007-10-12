#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestDiagnosticServiceFactory
    {
        internal static DiagnosticService CreateDiagnosticService()
        {
            Modality m = new Modality("01", "CT");

            HybridSet mpsTypes1 = new HybridSet();
            mpsTypes1.Add(new ModalityProcedureStepType("101", "CT Chest", m));
            mpsTypes1.Add(new ModalityProcedureStepType("102", "CT Abdo/Pelvis", m));

            HybridSet mpsTypes2 = new HybridSet();
            mpsTypes2.Add(new ModalityProcedureStepType("103", "MR Head", m));

            HybridSet rpTypes = new HybridSet();
            rpTypes.Add(new RequestedProcedureType("201", "CT Chest/Abdo/Pelvis"));
            rpTypes.Add(new RequestedProcedureType("202", "MR Head"));

            return new DiagnosticService("301", "R/O everything", rpTypes);
        }

        internal static DiagnosticService CreateDiagnosticService(int numReqProcs, int numMpsPerReqProc)
        {
            Modality m = new Modality("01", "CT");

            HybridSet procedures = new HybridSet();
            for (int p = 0; p < numReqProcs; p++)
            {
                HybridSet steps = new HybridSet();
                for(int s = 0; s < numMpsPerReqProc; s++)
                {
                    steps.Add(new ModalityProcedureStepType("10" + (s + p * numMpsPerReqProc), "MPS 10" + (s + p * numMpsPerReqProc), m));
                }
                procedures.Add(new RequestedProcedureType("20" + p, "Procedure 20" + p, steps));
            }
            return new DiagnosticService("301", "Diagnostic Service 301", procedures);
        }
    }
}
