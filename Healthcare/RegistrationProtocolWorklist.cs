using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
    public abstract class RegistrationProtocolWorklist : Worklist
    {
        protected abstract RegistrationWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);
        protected abstract Type ProtocolStepType { get; }

        public override IList GetWorklistItems(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklistItems(ProtocolStepType, GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistItemCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklistItemCount(ProtocolStepType, GetQueryConditions(currentUserStaff), this);
        }
    }
}
