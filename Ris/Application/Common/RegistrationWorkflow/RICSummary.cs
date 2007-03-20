using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RICSummary : DataContractBase
    {
        public RICSummary(string rpName, string orderingPractitioner, string insurance, DateTime? mpsScheduledTime, string performingFacility)
        {
            this.RequestedProcedureName = rpName;
            this.OrderingPractitioner = orderingPractitioner;
            this.Insurance = insurance;
            this.ModalityProcedureStepScheduledTime = mpsScheduledTime;
            this.PerformingFacility = performingFacility;
        }

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string OrderingPractitioner;

        [DataMember]
        public string Insurance;

        [DataMember]
        public DateTime? ModalityProcedureStepScheduledTime;

        [DataMember]
        public string PerformingFacility;
    }
}
