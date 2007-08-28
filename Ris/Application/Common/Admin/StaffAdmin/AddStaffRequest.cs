using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class AddStaffRequest : DataContractBase
    {
        public AddStaffRequest(StaffDetail staffDetail)
        {
            this.StaffDetail = staffDetail;
        }

        [DataMember]
        public StaffDetail StaffDetail;
    }
}
