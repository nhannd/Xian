using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public class CancelOrderOperation
    {
        public void Execute(Order order, OrderCancelReasonEnum reason)
        {
            //TODO this is kinda weird - what's the point of having separate Cancel and Discontinue methods?
            if (order.Status == OrderStatus.SC)
                order.Cancel(reason);
            else if (order.Status == OrderStatus.IP)
                order.Discontinue(reason);
            else
                throw new WorkflowException(string.Format("Order with status {0} cannot be cancelled.", order.Status));
        }
    }
}
