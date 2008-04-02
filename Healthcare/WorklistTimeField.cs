using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public enum WorklistTimeField
    {
        None,
        OrderSchedulingRequestTime,
        ProcedureScheduledStartTime,
        ProcedureCheckInTime,
        ProcedureCheckOutTime,
        ProcedureStartTime,
        ProcedureEndTime,
        ProcedureStepCreationTime,
        ProcedureStepScheduledStartTime,
        ProcedureStepStartTime,
        ProcedureStepEndTime,
    }
}
