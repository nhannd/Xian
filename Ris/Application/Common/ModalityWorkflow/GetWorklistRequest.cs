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
        //TODO: using string for now, this should be replaced by Enum code
        [DataMember]
        public string ActivityStatus;

        [DataMember]
        public string PatientProfileAuthority;
    }
}
