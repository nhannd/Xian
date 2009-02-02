using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Helper class for manipulating the Image Availability Work Queue.
	/// </summary>
	public static class ImageAvailabilityWorkQueue
	{
		private const string ProcedureOIDKey = "ProcedureOID";
		public const string WorkQueueItemType = "ImageAvailability";

		/// <summary>
		/// Creates a new work item.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="expirationTime"></param>
		/// <returns></returns>
		public static WorkQueueItem CreateWorkQueueItem(Procedure p, TimeSpan expirationTime)
		{
			WorkQueueItem item = new WorkQueueItem(WorkQueueItemType);
			item.ExpirationTime = Platform.Time.Add(expirationTime);
			item.ExtendedProperties.Add(ProcedureOIDKey, p.GetRef().Serialize());

			return item;
		}

		/// <summary>
		/// Polls the queue for pending items.
		/// </summary>
		/// <param name="batchSize"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IList<WorkQueueItem> GetPendingItems(int batchSize, IPersistenceContext context)
		{
			return context.GetBroker<IWorkQueueItemBroker>().GetPendingItems(WorkQueueItemType, batchSize);
		}

		/// <summary>
		/// Gets the procedure associated with the specified work item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static Procedure GetProcedure(WorkQueueItem item, IPersistenceContext context)
		{
			EntityRef procedureRef = new EntityRef(item.ExtendedProperties[ProcedureOIDKey]);
			return context.Load<Procedure>(procedureRef, EntityLoadFlags.Proxy);
		}
	}
}
