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
        public CheckInTableItem(EntityRef rpRef, string rpName, DateTime? schedulingDate, string orderingFacility)
        {
            this.RequestedProcedureRef = rpRef;
            this.RequestedProcedureName = rpName;
            this.SchedulingDate = schedulingDate;
            this.OrderingFacility = orderingFacility;
        }

        [DataMember]
        public EntityRef RequestedProcedureRef;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public DateTime? SchedulingDate;

        [DataMember]
        public string OrderingFacility;
    }
}
