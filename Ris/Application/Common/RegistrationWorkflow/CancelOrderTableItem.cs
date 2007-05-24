using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CancelOrderTableItem : DataContractBase
    {
        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string RequestedProcedureNames;

        [DataMember]
        public DateTime? SchedulingRequestDate;

        [DataMember]
        public EnumValueInfo Priority;
    }
}
