using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public interface IWorklistItem
    {
        string WorklistClassName { get; set; }
    }

    public interface IWorklist
    {
        IList GetWorklist(IPersistenceContext context);
    }

}
