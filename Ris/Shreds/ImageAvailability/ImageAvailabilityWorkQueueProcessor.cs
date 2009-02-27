using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes the Image Availability work queue.
	/// </summary>
	public class ImageAvailabilityWorkQueueProcessor : WorkQueueProcessor
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

		protected override string WorkQueueItemType
		{
			get { return ImageAvailabilityWorkQueue.WorkQueueItemType; }
		}

		protected override void ActOnItem(WorkQueueItem item)
		{
			Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, PersistenceScope.CurrentContext);
			procedure.ImageAvailability = _imageAvailabilityStrategy.ComputeProcedureImageAvailability(procedure, PersistenceScope.CurrentContext);
		}

		protected override void OnItemSucceeded(WorkQueueItem item)
		{
			// this method is overridden because image availability work items are never considered complete until they expire

			Procedure procedure = ImageAvailabilityWorkQueue.GetProcedure(item, PersistenceScope.CurrentContext);
			DateTime nextPollTime = Platform.Time.Add(GetPollingInterval(procedure.ImageAvailability));
			if(nextPollTime < item.ExpirationTime)
			{
				item.Reschedule(nextPollTime);
			}
			else
			{
				base.OnItemSucceeded(item);
			}
		}

		protected override bool ShouldRetry(WorkQueueItem item, Exception error, out DateTime retryTime)
		{
			// retry unless expired
			retryTime = Platform.Time.AddSeconds(_settings.PollingIntervalForError);
			return (retryTime < item.ExpirationTime);
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
