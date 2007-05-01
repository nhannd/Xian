using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Workflow
{
    public class Operations
    {
        public abstract class Operation
        {

        }

        public class CheckIn : Operation
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

        public class Cancel : Operation
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
