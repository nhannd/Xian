using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ModalityPerformedProcedureStep : PerformedProcedureStep
    {
        public virtual void Complete()
        {
            this.EndTime = Platform.Time;
            this.Status = ActivityPerformedStepStatus.CM;
        }

        public virtual void Discontinue()
        {
            this.EndTime = Platform.Time;
            this.Status = ActivityPerformedStepStatus.DC;
        }
    }
}
