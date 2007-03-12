using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class FindStaffsResponse : DataContractBase
    {
        public FindStaffsResponse(List<StaffSummary> staffs)
        {
            this.Staffs = staffs;
        }

        [DataMember]
        List<StaffSummary> Staffs;
    }
}
