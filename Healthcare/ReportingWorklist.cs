using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingWorklist : Worklist
    {
        protected abstract ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);
        protected abstract Type ProcedureStepType { get;}

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IReportingWorklistBroker>(context).GetWorklist(ProcedureStepType, GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IReportingWorklistBroker>(context).GetWorklistCount(ProcedureStepType, GetQueryConditions(currentUserStaff), this);
        }
    }

}
