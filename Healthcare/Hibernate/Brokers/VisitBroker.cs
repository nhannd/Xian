using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class VisitBroker
	{
		public IList<Visit> FindByPractitioner(ExternalPractitioner practitioner)
		{
			var q = this.GetNamedHqlQuery("visitsForPractitioner");
			q.SetParameter(0, practitioner);
			return CollectionUtils.Unique(q.List<Visit>());
		}
	}
}
