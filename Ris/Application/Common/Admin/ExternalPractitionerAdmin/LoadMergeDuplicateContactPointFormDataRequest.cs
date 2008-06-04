using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class LoadMergeDuplicateContactPointFormDataRequest : DataContractBase
	{
		public LoadMergeDuplicateContactPointFormDataRequest(ExternalPractitionerContactPointSummary contactPoint)
		{
			this.ContactPoint = contactPoint;
		}

		[DataMember]
		public ExternalPractitionerContactPointSummary ContactPoint;
	}
}
