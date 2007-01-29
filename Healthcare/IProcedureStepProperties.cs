using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public interface IProcedureStepProperties
    {
        Type ProcedureStepClass { get; }
        ActivityStatus Status { get; }
    }
}
