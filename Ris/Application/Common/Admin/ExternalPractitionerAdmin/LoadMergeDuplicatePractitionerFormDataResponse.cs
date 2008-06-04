using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeDuplicatePractitionerFormDataResponse : DataContractBase
	{
		public LoadMergeDuplicatePractitionerFormDataResponse(
			List<OrderSummary> affectedOrders,
			List<VisitSummary> affectedVisits)
		{
			this.AffectedOrders = affectedOrders;
			this.AffectedVisits = affectedVisits;
		}

		[DataMember]
		public List<OrderSummary> AffectedOrders;

		[DataMember]
		public List<VisitSummary> AffectedVisits;
	}
}
