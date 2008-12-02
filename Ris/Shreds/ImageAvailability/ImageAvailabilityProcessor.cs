using System;
using System.Collections.Generic;
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
			item.ExpirationTime = DateTime.Now.Add(expirationTime);
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
            :base(settings.BatchSize, TimeSpan.FromSeconds(settings.SleepDurationInSeconds))
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
					TimeSpan expirationTime = TimeSpan.FromHours(_settings.ExpirationTimeInHours);
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
            : base(settings.BatchSize, TimeSpan.FromSeconds(settings.SleepDurationInSeconds))
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
					procedure.ImageAvailability = imageAvailability;
					UpdateWorkQueueItem(item, procedure.ImageAvailability);
				}
				else
				{
					// fail the work item so that we record the failure
					item.Fail(error.Message);

					// reschedule the work item so that it will be retried
					item.Reschedule(DateTime.Now.AddMinutes(_settings.NextScheduledTimeForErrorInMinutes));
				}

				scope.Complete();
			}
		}

		private void UpdateWorkQueueItem(WorkQueueItem item, Healthcare.ImageAvailability imageAvailability)
		{
			switch (imageAvailability)
			{
				// ImageAvailability.X should never get pass into this method
				// case Healthcare.ImageAvailability.X:
				//     break;
				case Healthcare.ImageAvailability.N:
					item.Reschedule(DateTime.Now.AddMinutes(_settings.NextScheduledTimeForUnknownAvailabilityInMinutes));
					break;
				case Healthcare.ImageAvailability.Z:
					item.Reschedule(DateTime.Now.AddMinutes(_settings.NextScheduledTimeForZeroAvailabilityInMinutes));
					break;
				case Healthcare.ImageAvailability.P:
					item.Reschedule(DateTime.Now.AddMinutes(_settings.NextScheduledTimeForPartialAvailabilityInMinutes));
					break;
				case Healthcare.ImageAvailability.C:
					item.Complete();
					break;
				default:
					break;
			}
		}
	}
}
