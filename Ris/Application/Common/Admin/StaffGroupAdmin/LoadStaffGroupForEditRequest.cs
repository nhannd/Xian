using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class LoadStaffGroupForEditRequest : DataContractBase
    {
        public LoadStaffGroupForEditRequest(EntityRef staffGroupRef)
        {
            this.StaffGroupRef = staffGroupRef;
        }

        [DataMember]
        public EntityRef StaffGroupRef;
    }
}
