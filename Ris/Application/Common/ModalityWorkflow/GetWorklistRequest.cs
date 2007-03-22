using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistRequest : DataContractBase
    {
        [DataMember]
        public ModalityWorklistSearchData SearchCriteria;

        [DataMember]
        public string PatientProfileAuthority;
    }
}
