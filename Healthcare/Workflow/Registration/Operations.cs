using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using System.Collections;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    public class Operations
    {
        public abstract class RegistrationOperation
        {
            // nothing here
        }

        public class CheckIn : RegistrationOperation
        {
            public void Execute(EntityRef rpRef, Staff currentUserStaff, IPersistenceContext context)
            {
                RequestedProcedure rp = context.GetBroker<IRequestedProcedureBroker>().Load(rpRef);

                CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                cps.Start(currentUserStaff);
                cps.Complete(currentUserStaff);

                rp.CheckInProcedureSteps.Add(cps);
                context.Lock(rp, DirtyState.Dirty);
            }
        }

        public class Cancel : RegistrationOperation
        {
            public void Execute(EntityRef orderRef, OrderCancelReason reason, IPersistenceContext context)
            {
                Order order = context.GetBroker<IOrderBroker>().Load(orderRef);
                order.Cancel(reason);
                context.Lock(order, DirtyState.Dirty);
            }
        }
    }
}
