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
    public class WorklistItem : WorklistItemBase
    {
        private EntityRef _procedureStepRef;

        public WorklistItem(EntityRef step)
        {
            _procedureStepRef = step;
        }

        public override bool Equals(object obj)
        {
            WorklistItem that = (WorklistItem)obj;
            return (that != null) &&
                (this.ProcedureStep.Equals(that.ProcedureStep));
        }

        public override int GetHashCode()
        {
            return this.ProcedureStep.GetHashCode();
        }

        #region Public Properties

        public EntityRef ProcedureStep
        {
            get { return _procedureStepRef; }
        }

        #endregion
    }
}
