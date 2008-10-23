using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.PerformingDocumentation
{
    [DataContract]
    public class LoadDataRequest : DataContractBase
    {
        public LoadDataRequest(EntityRef orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}
