using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        public ReportingProcedureStep()
        {

        }

        public ReportingProcedureStep(RequestedProcedure procedure)
            :base(procedure)
        {
        }
    }
}
