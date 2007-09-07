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
