using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IExternalPractitionerContactPointBroker
	{
		void MergeContactPoints(ExternalPractitionerContactPoint duplicate, ExternalPractitionerContactPoint original);
		IList<Order> GetRelatedOrders(ExternalPractitionerContactPoint contactPoint);
	}
}
