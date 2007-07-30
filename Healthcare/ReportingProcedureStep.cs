using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        private ReportPart _reportPart;

        public ReportingProcedureStep()
        {
        }

        public ReportingProcedureStep(RequestedProcedure procedure, ReportPart reportPart)
            :base(procedure)
        {
            _reportPart = reportPart;
        }

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            : this(previousStep.RequestedProcedure, previousStep.ReportPart)
        {
        }

        public ReportPart ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

        public override void Discontinue()
        {
            if (this.ReportPart != null)
                this.ReportPart.Cancelled();

            base.Discontinue();
        }

    }
}
