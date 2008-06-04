using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeDuplicateContactPointFormDataResponse : DataContractBase
	{
		public LoadMergeDuplicateContactPointFormDataResponse(List<OrderSummary> affectedOrders)
		{
			this.AffectedOrders = affectedOrders;
		}

		[DataMember]
		public List<OrderSummary> AffectedOrders;
	}
}
