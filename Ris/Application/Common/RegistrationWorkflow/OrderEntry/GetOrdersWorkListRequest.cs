using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetOrdersWorkListRequest : DataContractBase
    {
        public GetOrdersWorkListRequest(string patientProfileAuthority)
        {
            //this.SearchCriteria = searchCriteria;
            this.PatientProfileAuthority = patientProfileAuthority;
        }

        //TODO: ModalityProcedureStepSearchCriteria
        //[DataMember]
        //public ModalityProcedureStepSearchCriteria SearchCriteria

        [DataMember]
        public string PatientProfileAuthority;
    }
}
