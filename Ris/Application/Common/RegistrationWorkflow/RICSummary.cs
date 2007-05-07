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
        public RICSummary(string rpName, 
            PersonNameDetail orderingPractitioner, 
            string insurance, 
            DateTime? mpsScheduledTime, 
            string performingFacility,
            string status)
        {
            this.RequestedProcedureName = rpName;
            this.OrderingPractitioner = orderingPractitioner;
            this.Insurance = insurance;
            this.ModalityProcedureStepScheduledTime = mpsScheduledTime;
            this.PerformingFacility = performingFacility;
            this.Status = status;
        }

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public PersonNameDetail OrderingPractitioner;

        [DataMember]
        public string Insurance;

        [DataMember]
        public DateTime? ModalityProcedureStepScheduledTime;

        [DataMember]
        public string PerformingFacility;

        [DataMember]
        public string Status;
    }
}
