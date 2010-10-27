#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.OrderEntry
{
	public class CancelOrDiscontinueOrderOperation
	{
		/// <summary>
		/// Executes Cancel Order operation.
		/// Checks if order is in scheduling state, then executes if it is.
		/// Otherwise, throws a WorkflowException.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="info"></param>
		public void Execute(Order order, OrderCancelInfo info)
		{
			if (order.Status == OrderStatus.SC)
				order.Cancel(info);
			else if (order.Status == OrderStatus.IP)
				order.Discontinue(info);
			else
				throw new WorkflowException(string.Format("Order with status {0} cannot be cancelled/discontinued.", order.Status));
		}

		/// <summary>
		/// Determines if cancelling an order is possible.
		/// The order needs to currently be in scheduling.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public bool CanExecute(Order order)
		{
			return order.Status == OrderStatus.SC || order.Status == OrderStatus.IP;
		}
	}

	public class AssignPlacerOrderNumberOperation
	{
		public void Execute(Order order, string placerOrderNumber)
		{
			if(!string.IsNullOrEmpty(order.PlacerNumber))
				throw new WorkflowException("Order already has a placer order number");

			order.PlacerNumber = placerOrderNumber;
		}

		public bool CanExecute(Order order)
		{
			return string.IsNullOrEmpty(order.PlacerNumber);
		}
	}
}
