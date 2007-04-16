using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CancelOrderRequest : DataContractBase
    {
        public CancelOrderRequest(List<EntityRef> cancelledOrders, EnumValueInfo cancelReason)
        {
            this.CancelledOrders = cancelledOrders;
            this.CancelReason = cancelReason;
        }

        [DataMember]
        public List<EntityRef> CancelledOrders;

        [DataMember]
        public EnumValueInfo CancelReason;
    }
}
