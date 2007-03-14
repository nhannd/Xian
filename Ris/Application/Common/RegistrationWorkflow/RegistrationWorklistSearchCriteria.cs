using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RegistrationWorklistSearchCriteria : DataContractBase
    {
        //TODO: expand the MPS search criteria here
        [DataMember]
        public string ActivityStatus;
    }
}
