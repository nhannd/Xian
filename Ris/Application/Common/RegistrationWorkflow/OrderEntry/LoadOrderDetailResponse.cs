using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class LoadOrderDetailResponse : DataContractBase
    {
        public LoadOrderDetailResponse(OrderDetail orderDetail)
        {
            this.OrderDetail = orderDetail;
        }

        [DataMember]
        public OrderDetail OrderDetail;
    }
}
