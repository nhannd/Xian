using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	public class ImageAvailabilityProcedureProcessor : QueueProcessor<Procedure>
	{
		private IImageAvailabilityUpdateStrategy _updateStrategy;
        private ImageAvailabilityShredSettings _settings;

        internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
            :base(settings.BatchSize, TimeSpan.FromSeconds(settings.SleepDurationInSeconds))
		{
            _settings = settings;
			try
			{
				_updateStrategy = (IImageAvailabilityUpdateStrategy)(new ImageAvailabilityUpdateStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_updateStrategy = new DefaultImageAvailabilityUpdateStrategy();
			}
		}

		protected override IList<Procedure> GetNextBatch(int batchSize)
		{
			IList<Procedure> procedures;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					// Find a list of procedures that match the criteria
					ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
					criteria.Status.EqualTo(ProcedureStatus.IP);
					criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

					SearchResultPage page = new SearchResultPage(0, batchSize);
					procedures = scope.Context.GetBroker<IProcedureBroker>().Find(criteria, page);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfProcedures", e);
					procedures = new List<Procedure>();
				}
			}

			return procedures;
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
					_updateStrategy.ScheduleWorkQueueItem(procedure, context);

					// Set this to Not Available so the worklist item doesn't get scheduled again for this procedure
                    procedure.ImageAvailability = Healthcare.ImageAvailability.N;
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.ScheduleWorkQueueItems", e);
				}

				scope.Complete();
			}
		}
	}

    public class ImageAvailabilityWorkQueueItemProcessor : QueueProcessor<WorkQueueItem>
	{
		private IImageAvailabilityUpdateStrategy _updateStrategy;
        private ImageAvailabilityShredSettings _settings;

        internal ImageAvailabilityWorkQueueItemProcessor(ImageAvailabilityShredSettings settings)
            : base(settings.BatchSize, TimeSpan.FromSeconds(settings.SleepDurationInSeconds))
		{
            _settings = settings;
			try
			{
				_updateStrategy = (IImageAvailabilityUpdateStrategy)(new ImageAvailabilityUpdateStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_updateStrategy = new DefaultImageAvailabilityUpdateStrategy();
			}
	    }

		protected override IList<WorkQueueItem> GetNextBatch(int batchSize)
		{
			IList<WorkQueueItem> items;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					items = scope.Context.GetBroker<IWorkQueueItemBroker>().GetPendingItems(_updateStrategy.ScheduledWorkQueueItemType, batchSize);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfWorkQueueItems", e);
					items = new List<WorkQueueItem>();
				}
			}

			return items;
		}

		protected override void ProcessItem(WorkQueueItem item)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)scope.Context;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				try
				{
					context.Lock(item);
					_updateStrategy.Update(item, context);
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.ProcessWorkQueueItems", e);
					item.Fail(e.Message);
					item.Reschedule(DateTime.Now.AddMinutes(_settings.NextScheduledTimeForErrorInMinutes));
				}

				scope.Complete();
			}
		}
	}
}
