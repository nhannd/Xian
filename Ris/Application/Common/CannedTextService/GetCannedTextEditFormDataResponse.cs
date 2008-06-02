using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class GetCannedTextEditFormDataResponse : DataContractBase
	{
		public GetCannedTextEditFormDataResponse(StaffSummary staff, List<StaffGroupSummary> staffGroups)
		{
			this.CurrentStaff = staff;
			this.StaffGroups = staffGroups;
		}

		[DataMember]
		public StaffSummary CurrentStaff;

		[DataMember]
		public List<StaffGroupSummary> StaffGroups;
	}
}
