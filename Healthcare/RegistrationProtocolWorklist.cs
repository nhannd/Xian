using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    public abstract class RegistrationProtocolWorklist : Worklist
    {
        protected abstract RegistrationWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);
        protected abstract Type ProtocolStepType { get; }

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklist(ProtocolStepType, GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklistCount(ProtocolStepType, GetQueryConditions(currentUserStaff), this);
        }
    }
}
