using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    public abstract class TechnologistWorklist : Worklist
    {
        protected abstract ModalityWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);
        protected abstract Type ProcedureStepType { get; }

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IModalityWorklistBroker>(context).GetWorklist(typeof(ModalityProcedureStep), GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IModalityWorklistBroker>(context).GetWorklistCount(typeof(ModalityProcedureStep), GetQueryConditions(currentUserStaff), this);
        }
    }
}
