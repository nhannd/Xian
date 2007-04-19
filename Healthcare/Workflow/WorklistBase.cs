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
        protected string _worklistClassName;

        #region IWorklistItem Members

        public string WorklistClassName
        {
            get { return _worklistClassName; }
            set { _worklistClassName = value; }
        }

        #endregion
    }

    public abstract class WorklistBase : IWorklist
    {
        #region IWorklist Members

        public virtual IList GetWorklist(IPersistenceContext context)
        { return null; }

        #endregion
    }
}
