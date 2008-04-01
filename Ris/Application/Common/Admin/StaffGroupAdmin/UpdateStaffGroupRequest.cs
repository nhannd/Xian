using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class UpdateStaffGroupRequest : DataContractBase
    {
        public UpdateStaffGroupRequest(StaffGroupDetail staffGroup)
        {
            this.StaffGroup = staffGroup;
        }

        [DataMember]
        public StaffGroupDetail StaffGroup;
    }
}
