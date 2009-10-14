using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
	[DataContract]
	public class GetWorklistEditFormChoicesResponse : DataContractBase
	{
		[DataMember]
		public List<WorklistClassSummary> WorklistClasses;

		[DataMember]
		public List<StaffSummary> StaffChoices;

		[DataMember]
		public List<StaffGroupSummary> GroupSubscriberChoices;

		[DataMember]
		public List<FacilitySummary> FacilityChoices;

		[DataMember]
		public List<LocationSummary> PatientLocationChoices;

		[DataMember]
		public List<EnumValueInfo> PatientClassChoices;

		[DataMember]
		public List<EnumValueInfo> OrderPriorityChoices;

		[DataMember]
		public List<StaffGroupSummary> OwnerGroupChoices;
	}
}
