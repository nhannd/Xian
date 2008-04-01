using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class AddStaffGroupRequest : DataContractBase
    {
        public AddStaffGroupRequest(StaffGroupDetail staffGroup)
        {
            this.StaffGroup = staffGroup;
        }

        [DataMember]
        public StaffGroupDetail StaffGroup;
    }
}
