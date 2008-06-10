using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
	[DataContract]
	public class AddStaffGroupsRequest : DataContractBase
	{
		public AddStaffGroupsRequest(List<StaffGroupSummary> staffGroups)
		{
			StaffGroups = staffGroups;
		}

		[DataMember]
		public List<StaffGroupSummary> StaffGroups;
	}
}
