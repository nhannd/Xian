using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetOrderRequisitionForEditResponse : DataContractBase
    {
        public GetOrderRequisitionForEditResponse(EntityRef orderRef, OrderRequisition requisition)
        {
            this.OrderRef = orderRef;
            this.Requisition = requisition;
        }

        [DataMember]
        public OrderRequisition Requisition;

        [DataMember]
        public EntityRef OrderRef;
    }
}
