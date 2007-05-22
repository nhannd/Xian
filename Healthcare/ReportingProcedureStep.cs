using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        private Report _report;

        public ReportingProcedureStep()
        {
        }

        public ReportingProcedureStep(RequestedProcedure procedure, Report report)
            :base(procedure)
        {
            _report = report;
        }

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            :this(previousStep.RequestedProcedure, previousStep.Report)
        {
        }

        public Report Report
        {
            get { return _report; }
            set { _report = value; }
        }
    }
}
