using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetOperationEnablementRequest : DataContractBase
    {
        public GetOperationEnablementRequest(ModalityWorklistItem item)
        {
            this.WorklistItem = item;
        }

        [DataMember]
        public ModalityWorklistItem WorklistItem;
    }
}
