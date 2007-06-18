using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderSummary : DataContractBase
    {
        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public DateTime? EnteredDateTime;

        [DataMember]
        public DateTime? SchedulingRequestDateTime;

        [DataMember]
        public StaffDetail OrderingPractitioner;

        [DataMember]
        public string OrderingFacility;

        [DataMember]
        public string ReasonForStudy;

        [DataMember]
        public EnumValueInfo OrderPriority;
    }
}
