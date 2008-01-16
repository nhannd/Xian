using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
    public abstract class ReportingWorklist : Worklist
    {
        protected abstract ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);
        protected abstract Type ProcedureStepType { get;}

        public override IList GetWorklistItems(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IReportingWorklistBroker>(context).GetWorklistItems(ProcedureStepType, GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistItemCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IReportingWorklistBroker>(context).GetWorklistItemCount(ProcedureStepType, GetQueryConditions(currentUserStaff), this);
        }
    }

}
