#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Alerts;

namespace ClearCanvas.Healthcare.Extended
{
	[ExtensionOf(typeof(OrderAlertExtensionPoint))]
	public class InvalidVisitAlert : OrderAlertBase
	{
		public override string Id
		{
			get { return "InvalidVisitAlert"; }
		}

		public override AlertNotification Test(Order order, IPersistenceContext context)
		{
			var reasons = new List<string>();
			if (order.Visit == null)
			{
				// This should never happen in production because an order must have a visit
				reasons.Add("This order is missing a visit");
			}
			else
			{
				// Check Visit status
				if (order.Visit.Status != VisitStatus.AA)
					reasons.Add("Visit Status is not active");

				// Check Visit date
				if (order.Visit.AdmitTime == null)
				{
					// This should never happen in production since visit admit date should always be created from HIS
					reasons.Add("Visit date is missing");
				}
				else if (order.ScheduledStartTime != null)
				{
					if (order.Visit.AdmitTime.Value.Date > order.ScheduledStartTime.Value.Date)
						reasons.Add("Visit date is in the future");
					else if (order.Visit.AdmitTime.Value.Date < order.ScheduledStartTime.Value.Date)
						reasons.Add("Visit date is in the past");
				}
			}

			if (reasons.Count > 0)
				return new AlertNotification(this.Id, reasons);

			return null;
		}
	}
}
