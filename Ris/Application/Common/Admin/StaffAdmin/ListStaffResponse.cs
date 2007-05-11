using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class ListStaffResponse : DataContractBase
    {
        public ListStaffResponse(List<StaffSummary> staffs)
        {
            this.Staffs = staffs;
        }

        [DataMember]
        public List<StaffSummary> Staffs;
    }
}
