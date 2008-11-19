using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class LoadOrderDocumentationDataRequest : DataContractBase
    {
        public LoadOrderDocumentationDataRequest(EntityRef orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}
