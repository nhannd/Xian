using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class LoadStaffGroupEditorFormDataResponse : DataContractBase
    {
        public LoadStaffGroupEditorFormDataResponse(List<StaffSummary> allStaff)
        {
            this.AllStaff = allStaff;
        }

        [DataMember]
        public List<StaffSummary> AllStaff;
    }
}
