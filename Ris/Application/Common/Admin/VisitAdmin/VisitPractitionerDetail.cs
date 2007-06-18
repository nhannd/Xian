using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class VisitPractitionerDetail : DataContractBase
    {
        [DataMember]
        public StaffSummary Practitioner;

        [DataMember]
        public EnumValueInfo Role;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;
    }
}
