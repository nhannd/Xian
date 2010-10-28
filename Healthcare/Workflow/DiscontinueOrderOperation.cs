#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
	[Obsolete("Use CancelOrDiscontinueOrderOperation instead")]
	public class DiscontinueOrderOperation
	{
        /// <summary>
        /// Executes Discontinue Order operation.
        /// Checks if order is in progress, then executes if it is.
        /// Otherwise, throws a WorkflowException.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="info"></param>
		public void Execute(Order order, OrderCancelInfo info)
		{
			if (order.Status == OrderStatus.IP)
				order.Discontinue(info);
			else
				throw new WorkflowException(string.Format("Order with status {0} cannot be discontinued.", order.Status));
		}

        /// <summary>
        /// Determines if discontinuing an order is possible.
        /// The order needs to currently be in progress.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
		public bool CanExecute(Order order)
		{
			return order.Status == OrderStatus.IP;
		}
	}
}
