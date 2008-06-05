using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IExternalPractitionerBroker
	{
		void MergePractitioners(ExternalPractitioner duplicate, ExternalPractitioner original);
		IList<Order> GetRelatedOrders(ExternalPractitioner practitioner);
		IList<Visit> GetRelatedVisits(ExternalPractitioner practitioner);
	}
}
