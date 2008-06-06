using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IExternalPractitionerBroker
	{
		IList<Order> GetRelatedOrders(ExternalPractitioner practitioner);
		IList<Visit> GetRelatedVisits(ExternalPractitioner practitioner);
	}
}
