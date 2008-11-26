using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	public interface IUpdateImageAvailabilityStrategy
	{
		string ScheduledWorkQueueItemType { get; }

		WorkQueueItem ScheduleWorkQueueItem(Procedure p, IPersistenceContext context);

		void Update(WorkQueueItem item, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class UpdateImageAvailabilityStrategyExtensionPoint : ExtensionPoint<IUpdateImageAvailabilityStrategy>
	{
	}
}
