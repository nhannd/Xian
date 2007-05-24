using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow
{
    public class Operations
    {
        public abstract class Operation
        {

        }

        public class CheckIn : Operation
        {
            public void Execute(Order o, Staff currentUserStaff, IWorkflow workflow)
            {
                CollectionUtils.ForEach<RequestedProcedure>(o.RequestedProcedures, new Action<RequestedProcedure>(
                    delegate(RequestedProcedure rp)
                    {
                        CheckInProcedureStep cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(rp.ProcedureSteps,
                                delegate(ProcedureStep step)
                                {
                                    return step is CheckInProcedureStep;
                                });

                        // The CPS should be created when each RP of an order is created, but just in case it's not
                        if (cps == null)
                        {
                            cps = new CheckInProcedureStep(rp);
                            workflow.AddActivity(cps);
                        }

                        cps.Start(currentUserStaff);
                    }));
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
