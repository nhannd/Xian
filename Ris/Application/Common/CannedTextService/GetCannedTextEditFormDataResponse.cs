using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class GetCannedTextEditFormDataResponse : DataContractBase
	{
		public GetCannedTextEditFormDataResponse(List<StaffGroupSummary> staffGroups)
		{
			this.StaffGroups = staffGroups;
		}

		[DataMember]
		public List<StaffGroupSummary> StaffGroups;
	}
}
