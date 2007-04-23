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
    }

    public interface IWorklist
    {
        IList GetWorklist(IPersistenceContext context);
        int GetWorklistCount(IPersistenceContext context);
    }

}
