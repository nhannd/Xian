using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class SearchPatientRequest : DataContractBase
    {
        public SearchPatientRequest(PatientProfileSearchData searchCriteria)
        {
            this.SearchCriteria = searchCriteria;
        }

        [DataMember]
        public PatientProfileSearchData SearchCriteria;
    }
}
