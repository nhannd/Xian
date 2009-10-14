using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
	[DataContract]
	public class GetWorklistEditValidationRequest : DataContractBase
	{
		public GetWorklistEditValidationRequest(bool isUserWorklist, StaffGroupSummary ownerGroup)
		{
			IsUserWorklist = isUserWorklist;
			OwnerGroup = ownerGroup;
		}

		[DataMember]
		public bool IsUserWorklist;

		[DataMember]
		public StaffGroupSummary OwnerGroup;
	}
}
