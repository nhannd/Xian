using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds
{
	public interface IImageAvailabilityUpdateStrategy
	{
		string ScheduledWorkQueueItemType { get; }

		WorkQueueItem ScheduleWorkQueueItem(Procedure p, IPersistenceContext context);

		void Update(WorkQueueItem item, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class ImageAvailabilityUpdateStrategyExtensionPoint : ExtensionPoint<IImageAvailabilityUpdateStrategy>
	{
	}
}
