#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Merge
{
	static class OrderMigrationPriority
	{
		public static void CompletedRecently(OrderSearchCriteria ordersWhere)
		{
			ordersWhere.Status.EqualTo(OrderStatus.CM);
			ordersWhere.EndTime.MoreThan(Platform.Time.AddDays(-30)); //TODO: define recently
		}

		public static void InProgress(OrderSearchCriteria ordersWhere)
		{
			ordersWhere.Status.EqualTo(OrderStatus.IP);
		}

		public static void Scheduled(OrderSearchCriteria ordersWhere)
		{
			ordersWhere.Status.EqualTo(OrderStatus.SC);
		}

		public static void All(OrderSearchCriteria ordersWhere)
		{
			// no filters
		}
	}

	static class VisitMigrationPriority
	{
		public static void All(VisitSearchCriteria visitsWhere)
		{
			// no filters
		}
	}
}
