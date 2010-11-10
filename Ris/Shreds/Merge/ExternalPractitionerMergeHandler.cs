#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using System.Collections.Generic;
using ClearCanvas.Healthcare.Brokers;
using System;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Merge handler implementation for merging instances of <see cref="ExternalPractitioner"/>.
	/// </summary>
	[ExtensionOf(typeof(MergeHandlerExtensionPoint))]
	public class ExternalPractitionerMergeHandler : MergeHandlerBase<ExternalPractitioner>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerMergeHandler()
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
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.CompletedRecently, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.CompletedRecently, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.InProgress, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.InProgress, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.Scheduled, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.Scheduled, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.All, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.All, MigrateOrder, context),
					(item, stage, context) => Migrate<Visit, VisitSearchCriteria>(item, stage, GetVisitBatch, VisitMigrationPriority.All, MigrateVisit, context),
					(item, stage, context) => DeletePractitioner(item, stage, context)
				};
			}
		}

		private static int DeletePractitioner(ExternalPractitioner practitioner, int stage, IPersistenceContext context)
		{
			try
			{
				// since there are no more referencing orders or visits, we can delete the practitioner
				// and its contact points
				context.GetBroker<IExternalPractitionerBroker>().Delete(practitioner);

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

		private static void MigrateOrder(ExternalPractitioner practitioner, Order order)
		{
			// update ordering practitioner
			order.OrderingPractitioner = practitioner.GetUltimateMergeDestination();

			// update result recipients
			foreach (var contactPoint in practitioner.ContactPoints)
			{
				foreach (var rr in order.ResultRecipients)
				{
					if (rr.PractitionerContactPoint.Equals(contactPoint))
						rr.PractitionerContactPoint = contactPoint.GetUltimateMergeDestination();
				}
			}
		}

		private static void MigrateVisit(ExternalPractitioner practitioner, Visit visit)
		{
			var dest = practitioner.GetUltimateMergeDestination();
			foreach (var visitPractitioner in visit.Practitioners)
			{
				if (visitPractitioner.Practitioner.Equals(practitioner))
					visitPractitioner.Practitioner = dest;
			}
		}

		private static IList<Order> GetOrderBatchByOrderingPractitioner(ExternalPractitioner practitioner, Action<OrderSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var ordersWhere = new OrderSearchCriteria();
			priorityFilter(ordersWhere);

			ordersWhere.OrderingPractitioner.EqualTo(practitioner);
			return context.GetBroker<IOrderBroker>().Find(ordersWhere, new SearchResultPage(0, batchSize));
		}

		private static IList<Order> GetOrderBatchByResultRecipient(ExternalPractitioner practitioner, Action<OrderSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var ordersWhere = new OrderSearchCriteria();
			priorityFilter(ordersWhere);

			var recipientWhere = new ResultRecipientSearchCriteria();
			recipientWhere.PractitionerContactPoint.In(practitioner.ContactPoints);

			return context.GetBroker<IOrderBroker>().FindByResultRecipient(ordersWhere, recipientWhere, new SearchResultPage(0, batchSize));
		}

		private static IList<Visit> GetVisitBatch(ExternalPractitioner practitioner, Action<VisitSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var visitsWhere = new VisitSearchCriteria();
			priorityFilter(visitsWhere);

			var practitionersWhere = new VisitPractitionerSearchCriteria();
			practitionersWhere.Practitioner.EqualTo(practitioner);
			return context.GetBroker<IVisitBroker>().FindByVisitPractitioner(new VisitSearchCriteria(), practitionersWhere, new SearchResultPage(0, batchSize));
		}
	}
}
