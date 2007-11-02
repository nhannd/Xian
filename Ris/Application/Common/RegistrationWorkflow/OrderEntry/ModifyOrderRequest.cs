using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ModifyOrderRequest : DataContractBase
    {
        public ModifyOrderRequest(EntityRef orderRef, OrderRequisition requisition)
        {
            this.OrderRef = orderRef;
            this.Requisition = requisition;
        }

        /// <summary>
        /// Order to modify.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Requisition specifying details of the modified order.
        /// </summary>
        [DataMember]
        public OrderRequisition Requisition;
    }
}