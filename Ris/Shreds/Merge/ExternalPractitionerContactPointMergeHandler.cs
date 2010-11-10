#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Merge handler implementation for merging instances of <see cref="ExternalPractitionerContactPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(MergeHandlerExtensionPoint))]
	public class ExternalPractitionerContactPointMergeHandler : MergeHandlerBase<ExternalPractitionerContactPoint>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ExternalPractitionerContactPointMergeHandler()
			:base(new MergeShredSettings().ItemsProcessedPerTransaction)
		{
		}

		/// <summary>
		/// Gets the set of merge steps to be performed.
		/// </summary>
		/// <remarks>
		/// Defines a set of migration steps to be executed. The first step in the list is always executed first.
		/// The execution of each step returns an integer indicating which step to execute next.
		/// </remarks>
		protected override MergeStep[] MergeSteps
		{
			get
			{
				return new MergeStep[]
				{
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.CompletedRecently, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.InProgress, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.Scheduled, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.All, MigrateOrder, context),
					(item, stage, context) => DeleteContactPoint(item, stage, context)
				};
			}
		}

		private static int DeleteContactPoint(ExternalPractitionerContactPoint contactPoint, int stage, IPersistenceContext context)
		{
			try
			{
				// since there are no more referencing orders or visits, we can delete the contact point
				context.GetBroker<IExternalPractitionerContactPointBroker>().Delete(contactPoint);

				// force the delete to occur, to ensure it will succeed
				context.SynchState();

				// merge completed
				return -1;
			}
			catch (PersistenceException)
			{
				// looks like the delete failed, most likely meaning that new references to the item
				// were introduced during the migration process
				// therefore we need to start over at stage 0
				return 0;
			}
		}

		private static void MigrateOrder(ExternalPractitionerContactPoint contactPoint, Order order)
		{
			// update result recipients
			var dest = contactPoint.GetUltimateMergeDestination();
			foreach (var rr in order.ResultRecipients)
			{
				if (rr.PractitionerContactPoint.Equals(contactPoint))
					rr.PractitionerContactPoint = dest;
			}
		}

		private static IList<Order> GetOrderBatchByResultRecipient(ExternalPractitionerContactPoint contactPoint, Action<OrderSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var ordersWhere = new OrderSearchCriteria();
			priorityFilter(ordersWhere);

			var recipientWhere = new ResultRecipientSearchCriteria();
			recipientWhere.PractitionerContactPoint.EqualTo(contactPoint);

			return context.GetBroker<IOrderBroker>().FindByResultRecipient(ordersWhere, recipientWhere, new SearchResultPage(0, batchSize));
		}
	}
}
