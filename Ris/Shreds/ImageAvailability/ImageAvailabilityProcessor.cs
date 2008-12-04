using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Helper class for manipulating the Image Availability Work Queue.
	/// </summary>
	public static class ImageAvailabilityWorkQueue
	{
		private const string ProcedureOIDKey = "ProcedureOID";
		private const string WorkQueueItemType = "ImageAvailability";

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

	/// <summary>
	/// Processes in-progress procedures, and schedules corresponding Image Availability work queue items.
	/// </summary>
	public class ImageAvailabilityProcedureProcessor : QueueProcessor<Procedure>
	{
        private readonly ImageAvailabilityShredSettings _settings;

        internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
            :base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
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

	/// <summary>
	/// Processes the Image Availability work queue.
	/// </summary>
    public class ImageAvailabilityWorkQueueItemProcessor : QueueProcessor<WorkQueueItem>
	{
		private readonly IImageAvailabilityStrategy _imageAvailabilityStrategy;
        private readonly ImageAvailabilityShredSettings _settings;

        internal ImageAvailabilityWorkQueueItemProcessor(ImageAvailabilityShredSettings settings)
            : base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
            _settings = settings;
			try
			{
				_imageAvailabilityStrategy = (IImageAvailabilityStrategy)(new ImageAvailabilityStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_imageAvailabilityStrategy = new DefaultImageAvailabilityStrategy();
			}
	    }

		protected override IList<WorkQueueItem> GetNextBatch(int batchSize)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					IList<WorkQueueItem> items = ImageAvailabilityWorkQueue.GetPendingItems(batchSize, scope.Context);

					scope.Complete();

					return items;
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityWorkQueueItemProcessor.GetNextBatch", e);
					throw;
				}
			}
		}

		protected override void ProcessItem(WorkQueueItem item)
		{
			Healthcare.ImageAvailability imageAvailability;
			Exception error = null;

			// compute the image availability in a read-scope, thus allowing the strategy
			// to query the model but not modify it
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				IReadContext context = (IReadContext)scope.Context;
				context.Lock(item);
				Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, context);
				imageAvailability = procedure.ImageAvailability;
				try
				{
					imageAvailability = _imageAvailabilityStrategy.ComputeProcedureImageAvailability(procedure, context);

					scope.Complete();
				}
				catch (Exception e)
				{
					// log exception
					ExceptionLogger.Log("ImageAvailabilityWorkQueueItemProcessor.ProcessItem", e);

					error = e;
				}
			}

			// create an update scope, to update the procedure and/or workqueue item
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)scope.Context;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				context.Lock(item);
				Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, context);

				if(error == null)
				{
                    // update the procedure
					procedure.ImageAvailability = imageAvailability;

                    // reschedule or complete the workitem
                    DateTime nextPollTime = Platform.Time.Add(GetPollingInterval(imageAvailability));
                    if (nextPollTime > item.ExpirationTime)
                    {
                        // item would expire prior to next poll time, so consider it complete
                        item.Complete();
                    }
                    else
                    {
                        // reschedule item
                        item.Reschedule(nextPollTime);
                    }
				}
				else
				{
					// fail the work item so that we record the failure
					item.Fail(error.Message);

					// reschedule the work item so that it will be retried
                    item.Reschedule(Platform.Time.AddSeconds(_settings.PollingIntervalForError));
				}

				scope.Complete();
			}
		}

        private TimeSpan GetPollingInterval(Healthcare.ImageAvailability imageAvailability)
        {
            switch (imageAvailability)
            {
                case Healthcare.ImageAvailability.N:
                    return TimeSpan.FromSeconds(_settings.PollingIntervalForIndeterminate);
                case Healthcare.ImageAvailability.Z:
                    return TimeSpan.FromSeconds(_settings.PollingIntervalForZero);
                case Healthcare.ImageAvailability.P:
                    return TimeSpan.FromSeconds(_settings.PollingIntervalForPartial);
                case Healthcare.ImageAvailability.C:
                    return TimeSpan.FromSeconds(_settings.PollingIntervalForComplete);
                default:
                    // ImageAvailability.X should never get pass into this method
                    throw new NotImplementedException();
            }
        }
	}
}
