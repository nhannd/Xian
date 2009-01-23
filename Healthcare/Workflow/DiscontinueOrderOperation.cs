using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
	public class DiscontinueOrderOperation
	{
		public void Execute(Order order, OrderCancelInfo info)
		{
			if (order.Status == OrderStatus.IP)
				order.Discontinue(info);
			else
				throw new WorkflowException(string.Format("Order with status {0} cannot be discontinued.", order.Status));
		}

		public bool CanExecute(Order order)
		{
			return order.Status == OrderStatus.IP;
		}
	}
}
