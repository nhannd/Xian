using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public class Operations
    {
        public abstract class Operation
        {

        }

        public class CheckIn : Operation
        {
            public void Execute(RequestedProcedure rp, Staff currentUserStaff, IWorkflow workflow)
            {
                CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                cps.Start(currentUserStaff);
                cps.Complete(currentUserStaff);
                workflow.AddActivity(cps);
            }
        }

        public class Cancel : Operation
        {
            public void Execute(Order order, OrderCancelReason reason)
            {
                order.Cancel(reason);
            }
        }
    }
}
