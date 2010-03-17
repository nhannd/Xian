using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeExternalPractitionerFormDataResponse : DataContractBase
	{
		[DataMember]
		public List<ExternalPractitionerSummary> Duplicates;

		[DataMember]
		public List<OrderDetail> AffectedOrders;
	}
}
