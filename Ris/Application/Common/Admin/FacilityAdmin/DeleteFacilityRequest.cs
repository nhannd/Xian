using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
	[DataContract]
	public class DeleteFacilityRequest : DataContractBase
	{
		public DeleteFacilityRequest(EntityRef facilityRef)
		{
			this.FacilityRef = facilityRef;
		}

		[DataMember]
		public EntityRef FacilityRef;
	}
}
