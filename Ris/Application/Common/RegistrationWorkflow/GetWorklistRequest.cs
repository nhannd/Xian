using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetWorklistRequest : DataContractBase
    {
        public GetWorklistRequest(string worklistClassName, PatientProfileSearchData searchCriteria)
        {
            this.WorklistClassName = worklistClassName;
            this.SearchCriteria = searchCriteria;
        }

        [DataMember]
        public string WorklistClassName;

        [DataMember]
        public PatientProfileSearchData SearchCriteria;
    }
}
