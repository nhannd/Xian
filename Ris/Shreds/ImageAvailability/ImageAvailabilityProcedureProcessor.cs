using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes in-progress procedures, and schedules corresponding Image Availability work queue items.
	/// </summary>
	public class ImageAvailabilityProcedureProcessor : QueueProcessor<Procedure>
	{
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
		}

		protected override IList<Procedure> GetNextBatch(int batchSize)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					// Find a list of procedures that match the criteria
					ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
					criteria.Status.EqualTo(ProcedureStatus.IP);
					criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

					SearchResultPage page = new SearchResultPage(0, batchSize);
					IList<Procedure> procedures = scope.Context.GetBroker<IProcedureBroker>().Find(criteria, page);

					scope.Complete();
					return procedures;
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcedureProcessor.GetNextBatch", e);
					throw;
				}
			}
		}

		protected override void ProcessItem(Procedure procedure)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)scope.Context;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				try
				{
					context.Lock(procedure);

					// create the workqueue item
					TimeSpan expirationTime = TimeSpan.FromHours(_settings.ExpirationTime);
					WorkQueueItem item = ImageAvailabilityWorkQueue.CreateWorkQueueItem(procedure, expirationTime);
					context.Lock(item, DirtyState.New);

					// Set this to Not Available so the worklist item doesn't get created again for this procedure
					procedure.ImageAvailability = Healthcare.ImageAvailability.N;

					// complete scope
					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcedureProcessor.ProcessItem", e);
					throw;
				}
			}
		}

	}
}
