using System.Collections.Generic;

namespace ClearCanvas.Workflow.Brokers
{
	public partial interface IWorkQueueItemBroker
	{
		IList<string> GetTypes();

		/// <summary>
		/// Gets pending work queue items of the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="maxItems"></param>
		IList<WorkQueueItem> GetPendingItems(string type, int maxItems);
	}
}
