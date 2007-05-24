using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CheckInTableItem : DataContractBase
    {
        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string RequestedProcedureNames;

        [DataMember]
        public DateTime? SchedulingDate;

        [DataMember]
        public string OrderingFacility;
    }
}
