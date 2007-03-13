using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistSearchCriteria : DataContractBase
    {
        //TODO: expand the MPS search criteria here
        [DataMember]
        public string ActivityStatus;
    }
}
