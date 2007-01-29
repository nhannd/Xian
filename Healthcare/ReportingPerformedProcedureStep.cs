using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public class ReportingPerformedProcedureStep : PerformedProcedureStep
    {
        public ReportingPerformedProcedureStep(Staff performingStaff)
            :base(performingStaff)
        {
        }

        public ReportingPerformedProcedureStep()
        {
        }

    }
}
