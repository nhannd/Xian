using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _procedureStep;

        public WorklistItemKey(EntityRef procedureStep)
        {
            _procedureStep = procedureStep;
        }

        public EntityRef ProcedureStep
        {
            get { return _procedureStep; }
            set { _procedureStep = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        public WorklistItem(EntityRef step)
            : base(new WorklistItemKey(step))
        {
        }

        #region Public Properties

        public EntityRef ProcedureStep
        {
            get { return (this.Key as WorklistItemKey).ProcedureStep; }
        }

        #endregion
    }
}
