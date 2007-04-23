using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public abstract class WorklistItemBase : IWorklistItem
    {

    }

    public abstract class WorklistBase : IWorklist
    {
        #region IWorklist Members

        public virtual IList GetWorklist(IPersistenceContext context)
        { return null; }

        public virtual int GetWorklistCount(IPersistenceContext context)
        { return -1; }

        #endregion
    }
}
