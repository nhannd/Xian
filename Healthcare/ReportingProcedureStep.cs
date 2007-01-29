using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

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

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            :base(previousStep.RequestedProcedure)
        {
            // todo: inherit report from previous step
        }
    }
}
