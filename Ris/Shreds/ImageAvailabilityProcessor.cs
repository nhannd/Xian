using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds
{
	public class ImageAvailabilityProcedureProcessor : ProcessorBase<Procedure>
	{
		private IImageAvailabilityUpdateStrategy _updateStrategy;

		public override void Initialize(int batchSize, int sleepDurationInSeconds)
		{
			try
			{
				_updateStrategy = (IImageAvailabilityUpdateStrategy)(new ImageAvailabilityUpdateStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_updateStrategy = new DefaultImageAvailabilityUpdateStrategy();
			}

			base.Initialize(batchSize, sleepDurationInSeconds);
		}

		public override IList<Procedure> GetNextBatch(int batchSize)
		{
			IList<Procedure> procedures;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					// Find a list of procedures that match the criteria
					ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
					criteria.Status.EqualTo(ProcedureStatus.IP);
					criteria.ImageAvailability.EqualTo(ImageAvailability.X);

					SearchResultPage page = new SearchResultPage(0, batchSize);
					procedures = scope.Context.GetBroker<IProcedureBroker>().Find(criteria, page);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfProcedures", e);
					scope.Dispose();
					procedures = new List<Procedure>();
				}
			}

			return procedures;
		}

		public override void ProcessItem(Procedure procedure)
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
					procedure.ImageAvailability = ImageAvailability.N;
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.ScheduleWorkQueueItems", e);
				}

				scope.Complete();
			}
		}
	}

	public class ImageAvailabilityWorkQueueItemProcessor : ProcessorBase<WorkQueueItem>
	{
		private IImageAvailabilityUpdateStrategy _updateStrategy;

		public override void Initialize(int batchSize, int sleepDurationInSeconds)
		{
			try
			{
				_updateStrategy = (IImageAvailabilityUpdateStrategy)(new ImageAvailabilityUpdateStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_updateStrategy = new DefaultImageAvailabilityUpdateStrategy();
			}

			base.Initialize(batchSize, sleepDurationInSeconds);
		}

		public override IList<WorkQueueItem> GetNextBatch(int batchSize)
		{
			IList<WorkQueueItem> items;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					DateTime now = DateTime.Now;

					// Find a list of procedures that match the criteria
					WorkQueueItemSearchCriteria criteria = new WorkQueueItemSearchCriteria();
					criteria.Type.EqualTo(_updateStrategy.ScheduledWorkQueueItemType);
					criteria.Status.EqualTo(WorkQueueStatus.PN);
					criteria.ExpirationTime.MoreThan(now);
					criteria.ScheduledTime.LessThan(now);

					SearchResultPage page = new SearchResultPage(0, batchSize);
					items = scope.Context.GetBroker<IWorkQueueItemBroker>().Find(criteria, page);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfWorkQueueItems", e);
					scope.Dispose();
					items = new List<WorkQueueItem>();
				}
			}

			return items;
		}

		public override void ProcessItem(WorkQueueItem item)
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

					ShredsSettings settings = new ShredsSettings();
					item.ScheduledTime = DateTime.Now.AddMinutes(settings.ImageAvailabilityNextScheduledTimeForErrorInMinutes);
					item.FailureCount++;
					item.FailureDescription = e.Message;
				}
				finally
				{
					item.ProcessedTime = DateTime.Now;
				}

				scope.Complete();
			}
		}
	}
}
