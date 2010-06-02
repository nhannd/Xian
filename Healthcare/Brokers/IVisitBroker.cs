using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IVisitBroker
	{
		IList<Visit> FindByPractitioner(ExternalPractitioner practitioner);
	}
}
