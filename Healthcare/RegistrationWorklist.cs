using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    public abstract class RegistrationWorklist : Worklist
    {
        protected abstract RegistrationWorklistItemSearchCriteria[] GetQueryConditions(Staff staff);

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetWorklist(GetQueryConditions(currentUserStaff), this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetWorklistCount(GetQueryConditions(currentUserStaff), this);
        }
    }
}
