using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
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
