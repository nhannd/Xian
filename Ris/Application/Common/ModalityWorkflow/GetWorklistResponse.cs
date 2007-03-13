using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistResponse : DataContractBase
    {
        public GetWorklistResponse(List<ModalityWorklistItem> worklistItems)
        {
            this.WorklistItems = worklistItems;
        }

        [DataMember]
        public List<ModalityWorklistItem> WorklistItems;
    }
}
