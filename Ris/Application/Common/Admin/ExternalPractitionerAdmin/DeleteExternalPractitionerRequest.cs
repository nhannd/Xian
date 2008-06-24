using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
	[DataContract]
	public class DeleteExternalPractitionerRequest : DataContractBase
	{
		public DeleteExternalPractitionerRequest(EntityRef practitionerRef)
		{
			this.PractitionerRef = practitionerRef;
		}

		[DataMember]
		public EntityRef PractitionerRef;
	}
}
