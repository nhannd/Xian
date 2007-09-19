using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CancelOrderRequest : DataContractBase
    {
        public CancelOrderRequest(EntityRef cancelledOrder, EnumValueInfo cancelReason)
        {
            this.CancelledOrders = new List<EntityRef>();
            this.CancelledOrders.Add(cancelledOrder);
            this.CancelReason = cancelReason;
        }

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
