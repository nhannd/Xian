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
        public CancelOrderTableItem(EntityRef orderRef, 
            string accessionNumber,
            string diagnosticServiceName,
            DateTime? schedulingRequestDate,
            EnumValueInfo priority)
        {
            this.OrderRef = orderRef;
            this.AccessionNumber = accessionNumber;
            this.DiagnosticServiceName = diagnosticServiceName;
            this.SchedulingRequestDate = schedulingRequestDate;
            this.Priority = priority;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public DateTime? SchedulingRequestDate;

        [DataMember]
        public EnumValueInfo Priority;
    }
}
