using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IExternalPractitionerContactPointBroker
	{
		IList<Order> GetRelatedOrders(ExternalPractitionerContactPoint contactPoint);
	}
}
