using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract class that roughly represents the notion of an IHE/DICOM General Purpose Performed Procedure Step (GP-PPS)
    /// </summary>
    public abstract class PerformedProcedureStep : PerformedStep
    {
        public PerformedProcedureStep(Staff performingStaff)
            : base(new ProcedureStepPerformer(performingStaff))
        {
        }

        public PerformedProcedureStep()
        {
        }
    }
}
