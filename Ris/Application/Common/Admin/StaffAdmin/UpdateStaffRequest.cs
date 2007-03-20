using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class UpdateStaffRequest : DataContractBase
    {
        public UpdateStaffRequest(EntityRef staffRef, StaffDetail staffDetail)
        {
            this.StaffRef = staffRef;
            this.StaffDetail = staffDetail;
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public StaffDetail StaffDetail;
    }
}
