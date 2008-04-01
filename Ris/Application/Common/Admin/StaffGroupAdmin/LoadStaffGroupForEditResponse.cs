using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class LoadStaffGroupForEditResponse : DataContractBase
    {
        public LoadStaffGroupForEditResponse(StaffGroupDetail staffGroup)
        {
            this.StaffGroup = staffGroup;
        }

        [DataMember]
        public StaffGroupDetail StaffGroup;
    }
}
