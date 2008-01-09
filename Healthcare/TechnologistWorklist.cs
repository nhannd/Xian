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

        public override IList GetWorklistItems(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IModalityWorklistBroker>(context).GetWorklistItems(typeof(ModalityProcedureStep), GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistItemCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IModalityWorklistBroker>(context).GetWorklistItemCount(typeof(ModalityProcedureStep), GetQueryConditions(currentUserStaff), this);
        }
    }
}
