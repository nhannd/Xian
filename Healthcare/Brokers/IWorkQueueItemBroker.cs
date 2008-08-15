using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IWorkQueueItemBroker
	{
		IList<string> GetTypes();
	}
}
