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
        public CancelOrderRequest(List<EntityRef> cancelledOrders, EnumValueInfo cancelReason, EntityRef staffRef)
        {
            this.CancelledOrders = cancelledOrders;
            this.CancelReason = cancelReason;
            this.Staff = staffRef;
        }

        [DataMember(IsRequired = true)]
        public List<EntityRef> CancelledOrders;

        [DataMember(IsRequired = true)]
        public EnumValueInfo CancelReason;

        [DataMember(IsRequired = true)]
        public EntityRef Staff;
    }
}
