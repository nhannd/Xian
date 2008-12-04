using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes the Image Availability work queue.
	/// </summary>
	public class ImageAvailabilityWorkQueueProcessor : QueueProcessor<WorkQueueItem>
	{
		private readonly IImageAvailabilityStrategy _imageAvailabilityStrategy;
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityWorkQueueProcessor(ImageAvailabilityShredSettings settings)
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

				if (error == null)
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
