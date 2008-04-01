using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class ListStaffGroupsResponse : DataContractBase
    {
        public ListStaffGroupsResponse(List<StaffGroupSummary> staffGroups)
        {
            this.StaffGroups = staffGroups;
        }

        [DataMember]
        public List<StaffGroupSummary> StaffGroups;
    }
}
