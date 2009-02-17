using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes in-progress procedures, and schedules corresponding Image Availability work queue items.
	/// </summary>
	public class ImageAvailabilityProcedureProcessor : EntityQueueProcessor<Procedure>
	{
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
		}

		protected override IList<Procedure> GetNextBatchCore(int batchSize)
		{
			// Find a list of procedures that match the criteria
			ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
			criteria.Status.EqualTo(ProcedureStatus.IP);
			criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

			SearchResultPage page = new SearchResultPage(0, batchSize);
			return PersistenceScope.CurrentContext.GetBroker<IProcedureBroker>().Find(criteria, page);
		}

		protected override void ActOnItemCore(Procedure procedure)
		{
			// create the workqueue item
			TimeSpan expirationTime = TimeSpan.FromHours(_settings.ExpirationTime);
			WorkQueueItem item = ImageAvailabilityWorkQueue.CreateWorkQueueItem(procedure, expirationTime);
			PersistenceScope.CurrentContext.Lock(item, DirtyState.New);
		}

		protected override void OnItemSucceeded(Procedure item)
		{
			// Set this to Not Available so the worklist item doesn't get created again for this procedure
			item.ImageAvailability = Healthcare.ImageAvailability.N;
		}

		protected override void OnItemFailed(Procedure item, Exception error)
		{
			// do nothing
		}
	}
}
